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
<<<<<<< HEAD
```
=======
```

### Install Bro

These steps can also be found in a repo [here](https://github.com/bah8892/NetworkMonitorVisConfig/blob/master/rpm/InstallBroScript.sh).

```
Update the system
yum update -y && yum upgrade -y
yum install git vim curl -y
```
Install Dependencies
```
yum install cmake make gcc gcc-c++ flex bison libpcap-devel openssl-devel python-devel swig zlib-devel git -y
```

Install Bro
```
cd /opt
git clone --recursive git://git.bro.org/bro
cd bro
./configure
make
make install
export PATH=/usr/local/bro/bin:$PATH
```

Configure Bro
```
broInterface=$(ip a | grep '2:' | grep 'en' | awk '{print $2}' |rev | cut -c 2- | rev)
echo $broInterface
sed -i "s#interface=eth0#interface=$broInterface#g" /usr/local/bro/etc/node.cfg
```

Enable Bro JSON logging
```
sed -i 's#const use_json = F#const use_json = T#g' /usr/local/bro/share/bro/base/frameworks/logging/writers/ascii.bro
```

In order for the bro logs to work with the visualization, we need to change the default delimiter of a “.” to a “_”, this makes them C# friendly. We can do this in file below: 

```
/usr/local/bro/share/bro/base/frameworks/logging/main.bro
```

List all your possibe interfaces for Bro:

```
ip a
```

Change the interface that Bro is monitoring here:
```
/usr/local/bro/etc/node.cfg
```

## Install Filebeat

Now that we have installed Bro, we can start sending logs to our Logstash server.

```
sudo rpm --import https://packages.elastic.co/GPG-KEY-elasticsearch
cat > /etc/yum.repos.d/elastic.repo << EOF
[elastic-5.x]
name=Elastic repository for 5.x packages
baseurl=https://artifacts.elastic.co/packages/5.x/yum
gpgcheck=1
gpgkey=https://artifacts.elastic.co/GPG-KEY-elasticsearch
enabled=1
autorefresh=1
type=rpm-md
EOF

sudo yum install filebeat -y
systemctl enable filebeat
service filebeat restart

mkdir /etc/filebeat/conf.d/
cp /etc/filebeat/filebeat.yml /etc/filebeat/filebeat.yml.bak

cat > /etc/filebeat/filebeat.yml << EOF
filebeat:
  registry_file: /var/lib/filebeat/registry
  config_dir: /etc/filebeat/conf.d

## Change this to whatever your logstash server is running on
output.logstash:
  hosts: ["$1:5044"]
EOF
```

After you make that basic configuration, go in and change the “hosts” line to whatever logstash host you are using. You can use multiple hosts if you want to, but I do not. 

Now we have to tell Filebeat what logs we want to forward. So just add this configuration file to the conf.d directory.

```
cat > /etc/filebeat/conf.d/bro.yml << EOF
filebeat.prospectors:
- input_type: log
  paths:
      - /usr/local/bro/logs/current/conn.log
      - /usr/local/bro/logs/current/dhcp.log
      - /usr/local/bro/logs/current/http.log
      - /usr/local/bro/logs/current/ssl.log
      - /usr/local/bro/logs/current/dns.log
      - /usr/local/bro/logs/current/known_services.log
      - /usr/local/bro/logs/current/ssh.log
      - /usr/local/bro/logs/current/weird.log
  fields:
    sensorID: $result
    sensorType: networksensor
  document_type: bro
EOF

systemctl restart filebeat

```
That will gather all the important bro logs for the visualization, and keep some of the grarbage out that Bro generates. If you DO want all the Bro logs sent to Logstash, just replace all of the file locations with the one below:

```
      - /usr/local/bro/logs/current/*.log
```

This will forward ALL .log files that bro generates to Logstash. 
>>>>>>> pages-updates
