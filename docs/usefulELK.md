# Useful tips and tricks for the ELK stack

#### Useful ELK stack commands
List all current indexes:
```
	curl -X GET “http://<IP addr of ES>:9200/_cat/indices?v”
```

Delete and index:
```
curl -XDELETE 'localhost:9200/.kibana?pretty'
```

# Useful CentOS 7 commands

List what services are running on what ports:
```
netstat -tnlp
```

Reload the firewalld
```
sudo firewall-cmd --reload	
```

Allow a http traffic through your firewall, permanently
```
sudo firewall-cmd --zone=public --add-service=http --permanent
```

Start a service
```
systemctl start <service>
```

Restart a service
```
systemctl restart <service>
```

Reset a failed service
```
systemctl reset-failed <service>
```

Use TCP Replay to replay a PCAP file on infinite loop, at 1 GB/s, on a specific interface
```
tcpreplay --loop=0 --mbps=100.0 --intf1=ens33 <Location of PCap> 
```

Start bro
```
/usr/local/bro/bin/broctl start
```

# Fixes to Common Problems with ELK

* Filebeat service is failing, this happens sometimes when you do not do a proper shutdown of either the filebeat service itself, or the CentOS machine. 
	* Remove the registry file 
	`rm -rf /var/lib/filebeat/registry`
	* Reset the failed service
	`systemctl reset-failed filebeat`
	* Restart the service
	`systemctl restart filebeat`

* Packetbeat seemingly being slow, after a large amount of traffic
	* Restart the service
	`systemctl restart packetbeat`
