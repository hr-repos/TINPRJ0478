from opcua import Client

print("\n OPC_UA TEST CLIENT \n")


client = Client("opc.tcp://127.0.0.1:8883")
client.connect()

namespaces = client.get_namespace_array()
print("namespaces: \t\t" + str(namespaces))

node_objects = client.get_objects_node()
node_objects_List = node_objects.get_children()

strNodes = "["
for node in node_objects_List:
    strNodes += str(node) + ", "
strNodes += "]"

print("node objects list: \t" + strNodes)

tempSensor = node_objects_List[1]
bulb = node_objects_List[2]
print("\ntempSensor: \t\t[" + str(tempSensor) + "]")
print("bulb: \t\t\t[]" + str(bulb) + "]")