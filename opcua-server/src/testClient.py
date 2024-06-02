from opcua import ua, Client, Node

client = Client("opc.tcp://145.24.223.210:8100")
client.connect()

namespaces = client.get_namespace_array()
print('namespaces: \t[', end='')
for index, namespace in enumerate(namespaces):
    if index == 0 or index == 1:
        continue
    
    message = namespace+'['+str(index)+']';
    if(index == len(namespaces)-1):
        print(message, end='')
    else:
        print(message, end=', ')
print(']')

nodes = client.get_objects_node().get_children()
print('nodes: \t\t[', end='')
for index, element in enumerate(nodes):
    #skip first node because that node is the server node
    if index == 0: 
        continue
    
    node: Node = element
    
    name = node.get_display_name().Text
    if(index == len(nodes)-1):
        print(name , end='')
    else:
        print(name , end=', ')
        
print(']')
    