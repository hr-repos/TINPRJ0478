from opcua import Client
client = Client("opc.tcp://127.0.0.1:8080")
client.connect()

namespaces = client.get_namespace_array()
print(namespaces)

node_objects = client.get_objects_node()
node_objects_List = node_objects.get_children()
print(node_objects_List)

tempSensor = node_objects_List[1]
bulb = node_objects_List[2]
print(tempSensor)
print(bulb)