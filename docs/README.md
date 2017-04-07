# Ergo Witness
3D Net is a Network Visualisation tool that shows network data in a creative way in 3D spaces. On the backend, the data is gathered by running tools like [Bro](https://www.bro.org/), [Packetbeat](https://www.elastic.co/products/beats/packetbeat), and [Snort](https://www.snort.org/), and pushing their logs in JSON format to the ELK stack (Elasticsearch). 

The ‘front end’ as I call it, is developed in the Unity game engine, pulling the information it needs down from the server with HTTP requests. By using a game engine like [Unity](https://unity3d.com/) to represent network data, there are not only possibilities of fun, interactive data analysis, but also the beginning of what could possibly be a very powerful Virtual Reality toolkit for network  professionals. 


[![Ergo Witness on Youtube](https://img.youtube.com/vi/UZcHY_7BzZY/0.jpg)](https://www.youtube.com/watch?v=UZcHY_7BzZY)

## [Prerequisites](prereqs.md)
## [How to Install and Configure Logstash](install_logstash.md)
## [How to Configure a Capture Server](captureServer.md)


## How to Install and Configure Logstash

Directions on how to install Logstash to get it to work with Ergo Witness are [here](install_logstash.md).

## How to Configure a Capture Server

Without a way to actually capture network data, this is pretty useless! [Here is a guide to set up a simple capture machine](captureServer.md). 