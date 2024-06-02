# mappenstructuur
Alles behalve `embedded` is inclusief Docker-scripts.
### aansturing
- `aansturing/webserver`: Het dashboard in Vue.
- `aansturing/Backend`: De backend van de aansturing in C#.
- `aansturing/mosquitto`: De MQTT-broker van de aansturing.
### control-centrum
- `control-centrum/ControlCentrum`: De backend van het control centrum in C#.
- `control-centrum/mosquitto`: De MQTT-broker van het control centrum.
### embedded
Voor de ESP32's. Deze kan geopend worden als PlatformIO-project.
### opcua-server
De OPC-UA-server die op een aparte Ubuntu-server wordt gerund, in Python.
### raspberry-pi
Alle commando's die zijn gerund om de Raspberry Pi te installeren.
