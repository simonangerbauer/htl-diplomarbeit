Initlialisation Script for CouchDB on OpenStack
===============================================

The Python scripts found in this folder can be used to provision a public CouchDB on top of Ubuntu 15.04 on a OpenStack Cloud. In order to roll out CouchDB the following prequisites need to be met:

  * Download OpenStack RC file and source it in local shell (i.e. `. ./admin-openrc.sh`)
  * Download and install the [Ubuntu 15.04 Cloud Image](https://cloud-images.ubuntu.com/vivid/current/vivid-server-cloudimg-amd64-disk1.img) as Raw image in Openstack. The image has to be named *Ubuntu 15.04 Server Cloud* and needs to be available to your project.
  * A security group named *CouchDB-ssh* with ingress ports 22 and 5984 open to the public needs to exist in your project (ask admin).
  * A network called *private* with capabilities to attach public floating IPs needs to exist

If you prequisites have differnt names just change in the header of `provision.py`.

The system can be provisioned through issuing `python provision.py`

The system can be torn down through `python teardown.py`

The scripts generate a ssh key which can be used to connect as user *ubuntu*. Keys are stored as *couchdb-ssh*.
