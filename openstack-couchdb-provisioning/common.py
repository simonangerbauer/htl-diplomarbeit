#!/usr/bin/env python

# Define colors for printing
class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


# Print OK Text
def print_ok(text):
	print bcolors.OKGREEN + text + bcolors.ENDC

def print_warning(text):
	print bcolors.WARNING + text + bcolors.ENDC
