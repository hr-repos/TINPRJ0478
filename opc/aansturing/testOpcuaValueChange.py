from opcua import Client, Node

cl = Client("opc.tcp://145.24.223.210:8100")
cl.connect()

node: Node = cl.get_node('ns=2;s="vkl1_mode"')
node.set_value(42)
print("changedValue: " + str(node.get_value()))
cl.disconnect()
exit()
