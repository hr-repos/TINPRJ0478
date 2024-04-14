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
vender = tempSensor.add_variable('ns=2;s="ValueID"', "TS1 VendorName", "value")
temp = tempSensor.add_variable('ns=2;s="TS1_Temp"', "TS1_Temp", 20)
vender.set_writable()
temp.set_writable()

bulb = nodes.add_object('ns=2;s="LB1"', "light bulb")
state = bulb.add_variable('ns=2;s="LB1_State"', "state of lightbulb", False)
brightness = bulb.add_variable('ns=2;s="LB1_Brightness"', "brightness", 100)
brightness.set_writable()
state.set_writable()

print("starting server...")
server.start()