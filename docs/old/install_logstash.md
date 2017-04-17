## Install Logstash

This section will be all about how to configure your ELK server to be compatible with the Ergo Witness front end. All of these commands are for RPM, but DEB guides can be found with a quick Google search if needed. I have a sh script that will do a basic installation of the ELK stack for you here. Take note that it is important to understand how ELK works in order to troubleshoot some things.

### Install Java

```
yum install wget -y

cd /opt

wget --no-cookies --no-check-certificate --header "Cookie: gpw_e24=http%3A%2F%2Fwww.oracle.com%2F; oraclelicense=accept-securebackup-cookie" "http://download.oracle.com/otn-pub/java/jdk/8u102-b14/jre-8u102-linux-x64.rpm"

rpm -Uvh jre-8u102-linux-x64.rpm
rm -rf jre-8u102-linux-x64.rpm
yum install java-devel -y

```



### Install Elasticsearch

```
sudo rpm --import https://artifacts.elastic.co/GPG-KEY-elasticsearch

echo '[elasticsearch-5.x]
name=Elasticsearch repository for 5.x packages
baseurl=https://artifacts.elastic.co/packages/5.x/yum
gpgcheck=1
gpgkey=https://artifacts.elastic.co/GPG-KEY-elasticsearch
enabled=1
autorefresh=1
type=rpm-md
' | sudo tee /etc/yum.repos.d/elasticsearch.repo
sudo yum -y install elasticsearch


```

Change the network host to localhost
```
sed -i 's/#network.host: 192.168.0.1/network.host: localhost/g' /etc/elasticsearch/elasticsearch.yml
```

Enable Elasticsearch to start when the machine boots, and then start Elasticsearch
```
systemctl enable elasticsearch
systemctl start elasticsearch
```

### Install Kibana

Now we will install Kibana, so that we can see our data

```
echo '[kibana-4.4]
name=Kibana repository for 4.4.x packages
baseurl=http://packages.elastic.co/kibana/4.4/centos
gpgcheck=1
gpgkey=http://packages.elastic.co/GPG-KEY-elasticsearch
enabled=1
' | sudo tee /etc/yum.repos.d/kibana.repo

sudo yum -y install kibana

sudo vi /opt/kibana/config/kibana.yml

```

In the kibana config file(/etc/kibana/kibana.yml), find the line that specifies server.host and replace the ip with "localhost", so that it looks like this:

```
## server.host: "localhost"
```

Now start kibana:
```
sudo systemctl start kibana
sudo chkconfig kibana on
```

### Install Logstash

```
echo '[logstash-5.x]
name=Elastic repository for 5.x packages
baseurl=https://artifacts.elastic.co/packages/5.x/yum
gpgcheck=1
gpgkey=https://artifacts.elastic.co/GPG-KEY-elasticsearch
enabled=1
autorefresh=1
type=rpm-md
' | sudo tee /etc/yum.repos.d/logstash.repo

sudo yum -y install logstash

systemctl restart logstash
systemctl enable logstash
```

### Configure Logstash

In order for Packetbeat and Filebeat to actually send information to Logstash, we need some configuration files for Logstash.

Make a configuration directory for logstash if you do not already have one:

```
mkdir /etc/logstash/conf.d
```


Copy my configuration folder from my GitHub into that new directory. Get those files [here](https://github.com/bah8892/NetworkMonitorVisConfig/tree/master/Configuration/logstash/conf.d)


Restart logstash
```
systemctl restart logstash
```



## [Prerequisites](prereqs.md)
## [How to Install and Configure Logstash](install_logstash.md)
## [How to Configure a Capture Server](captureServer.md)

## [Useful ELK and CentOS Commands](usefulELK.md)
## [Useful Guides](guides.md)
