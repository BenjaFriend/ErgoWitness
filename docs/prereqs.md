## Prerequisites

There are a couple things that you need in order for this visualisation to run
A CentOS 7 **  that is running Logstash and Elasticsearch, that you have configured and have access to.
For installation steps see the Appendix
A Windows 10 PC to run the visualisation

#### Recommended Specs
* 16 GB RAM
* Intel Core i7
* 2 GB graphics memory
* 250 GB SSD
* Internet access (ethernet cable prefered)

##### Min specs:
* 8 GB RAM
* Intel Core i3
* 1 GB graphics memory
* 500 MB free space on hard drive
* Internet access
* A CentOS 7 ** machine to capture the traffic with Bro and Packetbeat installed

#### For installation steps see the Appendix
The easiest way to set this up is to have a dual port NIC in this machine, and have a SPAN port coming out of the competition infrastructure. Then use that interface for Bro and Packetbeat.
Recommended Specs:
* Dual port gigabit NIC
* Intel core i7 / Inten Xeon E3 and above
* 32 GB RAM
* 1 TB SSD

** It should be noted that this machine can be on either CentOS 7, or Ubuntu. CentOS 7 is recommended because of it’s level of stability, and wider support for server software. All the commands for this guide will be in CentOS 7.


## [Prerequisites](prereqs.md)
## [How to Install and Configure Logstash](install_logstash.md)
## [How to Configure a Capture Server](captureServer.md)

## [Useful ELK and CentOS Commands](usefulELK.md)
## [Useful Guides](guides.md)
