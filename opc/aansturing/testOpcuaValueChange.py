from opcua import Client, Node

cl = Client("opc.tcp://localhost:8100")
cl.connect()

node: Node = cl.get_node('ns=2;s="vkl1_mode"')
node.set_value(0)
print("initialValue: " + str(node.get_value()))
node.set_value(1)
print("changedValue: " + str(node.get_value()))
node.set_value(0)
cl.disconnect()
exit()
