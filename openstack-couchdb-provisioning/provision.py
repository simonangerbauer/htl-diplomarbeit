#!/usr/bin/env python

#
# Python script based on the python-openstack (v2) library and tools
# Automatically deploys an Ubuntu 15.04 Server Cloud Image and installs CouchDB

import time
import subprocess
import os
import sys
import socket
import shutil

from common import *

from novaclient.v2 import client as nvclient
from credentials import get_nova_creds

# Parse credentials (as provided through an openrc file)
creds = get_nova_creds()
nova = nvclient.Client(**creds)

# Default values
keypair_name = 'couchdb-ssh'
server_image = 'Ubuntu 15.04 Server Cloud'
network_name = 'private'
server_flavor = 'm1.medium'
server_security_group = 'CouchDB-ssh'
cluster_info = '.couchdb.info'

# Create a Log file which is later to be used to tear down the server
cluster_info_file = open(cluster_info, 'w')
cluster_info_file.write('# This is a cluster-info file that describes a provisioned couchdb server\n')
cluster_info_file.write('# In the following all base configuration parameters are printed\n')
cluster_info_file.write('#\n')
cluster_info_file.write('# keypair_name: ' + keypair_name + '\n')
cluster_info_file.write('# server_image: ' + server_image + '\n')
cluster_info_file.write('# network_name: ' + network_name + '\n')
cluster_info_file.write('#\n#\n#\n')

# Function that can be used to store information to the cluster_info file
def write_cluster_info(key, value):
	cluster_info_file.write(key + ': ' + value + '\n')

	# Properly react to errors
class Error(Exception):
	pass

# Function that checks for existing SSH key-value pair
# if none was found just creates one
def ssh_keys(pvtkname):
	pubkname = pvtkname + '.pub'
	if os.path.isfile(pvtkname) and os.path.isfile(pubkname):
		print_ok('Found existing local keypair in ' + pvtkname + ', ' + pubkname)
	else:
		print_ok('Generating new keypair in ' + pvtkname + ', ' + pubkname)
		subprocess.check_call(['ssh-keygen', '-t', 'rsa', '-N', '', '-f', pvtkname])
	return dict({'private' : pvtkname, 'public' : pubkname})

# Can be used to find a floating ip
def find_floating_ip(wantedip, forceWanted=False):
	ips = nova.floating_ips.list()
	if wantedip is None:
		for cur_ip in ips:
			if cur_ip.instance_id is None:
				return cur_ip
	else:
		for cur_ip in ips:
			if cur_ip.ip == wantedip and cur_ip.instance_id is None:
				return cur_ip
			elif not forceWanted:
				return find_floating_ip(None)
	raise Error('Could not find a (or selected) floating IP')

# Function that creates a single server
def create_server(servername, imagename, networkname, flavorname, keypairname):
	image = nova.images.find(name=imagename)
	flavor = nova.flavors.find(name=flavorname)
	network = nova.networks.find(label=networkname)
	nics = [{'net-id': network.id}]
	server = nova.servers.create(name = servername,
		image = image.id,
		flavor = flavor.id,
		nics = nics,
		key_name = keypairname)
	write_cluster_info('server', server.id)
	return server

# Wait or server to switch to active state
def wait_for_server(server, network):
	while True:
		server = nova.servers.get(server.id)
		if server.status == 'ACTIVE':
			serverip = server.addresses[network][0]['addr']
			return serverip
		print_ok('Waiting for server ' + server.name + ' to become ACTIVE')
		time.sleep(15)

# Check for service ports beeing available
def probe_port(ip, port):
	sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	result = sock.connect_ex((ip, port))
	return result == 0

# Wait a minute for port becoming available
def wait_for_port(ip, port, tries = 5, info = True):
	while tries > 0:
		if probe_port(ip, port):
			return
		else:
			tries -= 1
			if info:
				print 'service ' + ip + ':' + str(port) + ' not availalbe waiting another 15sec'
			time.sleep(15)
	raise Error('Service ' + ip + ' at port ' + str(port) + ' did not become available in over a minute ... please check stack manually')

# Check for the security group we need
def find_security_group(sgname):
	sgs = nova.security_groups.list()
	for sg in sgs:
		if sg.name == sgname:
			return sg
	raise Error('Security group ' + sgname + ' not found please create the security group. We need port 22 (SSH) and 8080 (HTTP)')

# Execute a command on a remote machine
def execute_ssh(ip, private_key, command):
	try:
		subprocess.check_call(['ssh', '-q', '-t', '-i', private_key, '-o', 'StrictHostKeyChecking=no', 'ubuntu@' + ip, command])
	except:
		print 'Non zero exit code on ssh command: ' + command + ' please check your cluster manually'

# Copy a folder to remote machine through rsync
def rsync_folder(ip, private_key, local_folder, remote_folder):
	subprocess.check_call(['rsync', '-azh', local_folder, '-e', 'ssh -i ' + private_key + ' -o StrictHostKeyChecking=no', 'ubuntu@' + ip + ':' + remote_folder])

# Copy a single file to the remote machien through scp
def scp_file(ip, private_key, local_file, remote_folder):
	subprocess.check_call(['scp', '-i', private_key, '-o', 'StrictHostKeyChecking=no', local_file, 'ubuntu@' + ip + ':' + remote_folder ])

# Instructs the remote machine to grow its partition and reboot
def grow_partition(ip, private_key):
	execute_ssh(ip, private_key, 'sudo yum -y -q install cloud-utils-growpart')
	execute_ssh(ip, private_key, 'sudo growpart /dev/vda 1')
	execute_ssh(ip, private_key, 'sudo reboot')

# Set the hostname
def set_hostname(ip, private_key, hostname):
	execute_ssh(ip, private_key, 'sudo sed -i "s/HOSTNAME=.*/HOSTNAME=' + hostname + '/g" /etc/sysconfig/network')
	execute_ssh(ip, private_key, 'sudo hostname ' + hostname)


# Create SSH Keypair for VM access
keypair = None
avail_keypairs = nova.keypairs.list()
for cur_keypair in avail_keypairs:
	if cur_keypair.id == keypair_name:
		keypair = cur_keypair
if keypair is None:
	kp = ssh_keys(keypair_name)
	f = open(kp['public'],'r')
	pubkey = f.readline()[:-1]
	keypair = nova.keypairs.create(keypair_name, pubkey)
write_cluster_info('keypair', keypair.id)

# Check if image exists
try:
	image = nova.images.find(name=server_image)
except:
	raise Error('VM image "' + server_image + '" not found. Please make sure you have an image called "' + server_image + '" available on your OpenStack. The image can be downloaded from: https://cloud-images.ubuntu.com/vivid/current/vivid-server-cloudimg-amd64-disk1.img')

# Check if we can find a free floating-ip
server_public_ip = find_floating_ip(None)

# Check if the necessary security group is available
server_security_group = find_security_group(server_security_group)

# We made it until here, seems we are good to go
server = create_server('couchdb-runner-ubuntu', server_image, network_name, server_flavor, keypair_name)
server_private_ip = wait_for_server(server, network_name)
server.add_floating_ip(server_public_ip)
server.add_security_group(server_security_group.id)
wait_for_port(server_public_ip.ip, 22, 20, False)

# Server reachable through ssh --- connect and install
execute_ssh(server_public_ip.ip, keypair_name, 'sudo apt-get -y install couchdb')
execute_ssh(server_public_ip.ip, keypair_name, 'sudo sudo sed -i \'s/bind_address = 127\.0\.0\.1/bind_address = 0.0.0.0/g\' /etc/couchdb/default.ini')
execute_ssh(server_public_ip.ip, keypair_name, 'sudo service couchdb restart')

# Everything went fine inform user
print ''
print ''
print ''

print_ok('CouchDB was successfully installed:')
print_ok('http://' + server_public_ip.ip + ':5984/_utils')
