# from time import sleep
# import random
from opcua import Server

print("\n OPC_UA TEST SERVER \n")

server = Server()

server.set_endpoint("opc.tcp://127.0.0.1:8080")
nameSpaceStatus = server.register_namespace("Room1")
print(nameSpaceStatus)
 
nodes = server.get_objects_node()
print(nodes)

tempSensor = nodes.add_object('ns=2;s="TS1"', "Temperature Sensor 1")
tempSensor.add_variable('ns=2;s="TS1 VendorName"', "TS1 VendorName", "value")
temp = tempSensor.add_variable('ns=2;s="TS1 Temp"', "TS1 Temp", 20)

bulb = nodes.add_object(2, "light bulb")
state = bulb.add_variable(2, "state of lightbulb", False)
state.set_writable()

print("starting server...")
server.start()