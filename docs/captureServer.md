# Create a Capture Server

In order to actually send data to Logstash, we need a device that can grab the information that we want. For this project, I used Bro, Filebeat, and Packetbeat. Again,, these commands are for an RPM distribution of Linux. 


### Install Packetbeat

```
sudo yum install libpcap
curl -L -O https://artifacts.elastic.co/downloads/beats/packetbeat/packetbeat-5.2.2-x86_64.rpm
sudo rpm -vi packetbeat-5.2.2-x86_64.rpm
```

That's really it, from here you need to change the configuration to your liking. Something that you will definitely need to do is change the interface that your are listening on, and the output to Logstash. If you want a more real time visualization, also change the packetbeat flow reporting period to something small, like 1 second. 

If you need to know what interfaces you have available, you can use this command: 

```
ip a
```