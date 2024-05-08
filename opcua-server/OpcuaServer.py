import json
from time import sleep
from opcua import ua, Server

def getDescriptor(namespace: str, ID: str):
    return 'ns={};s=\"{}\"'.format(namespace, ID)

class UA_Variable():
    def __init__(self, varDescriptor : str, name : str, value, writeable : bool):
        self.VarDescriptor: str = varDescriptor
        self.writeable: bool = writeable
        self.name: str = name
        self.value = value
        
    def addVariable(self, node_reference):
        var = node_reference.add_variable(self.VarDescriptor, self.name, self.value)
        
        if self.writeable == True:
            var.set_writable()

class UA_Node():
    def __init__(self, namespace: str, ID: str, name: str):
        self.nodeDescriptor: str = getDescriptor(namespace, ID)
        self.namespace: str = namespace
        self.name: str = name
        
        self.vars: dict[str, UA_Variable]= {}
    
    def __init__(self, json: dict):
        self.nodeDescriptor: str = getDescriptor(json["namespace"], json["nodeID"])
        self.namespace: str = json["namespace"]
        self.name: str = json["name"]
        
        self.vars: dict[str, UA_Variable]= {}
        
        for var in json.get("variables", []):
            self.addVariable(var["id"], var["value"], var.get("writeable", True))
    
    def addVariable(self, idAndName : str, value, setWritable = True):
        varDescriptor = getDescriptor(self.namespace, idAndName)
        self.vars[idAndName] = UA_Variable(varDescriptor, idAndName, value, setWritable)
    
    def addToServer(self, server: Server) -> ua.NodeClass:
        nodeHandler = server.get_objects_node()
        node_reference = nodeHandler.add_object(self.nodeDescriptor, self.name)
        
        for variable in self.vars.values():
            variable.addVariable(node_reference)
            
        return node_reference

    def getName(self) -> str:
        return self.name
    
class UA_Server(): 
    def __init__(self, address: str, port: int, name: str):
        endpoint: str = "opc.tcp://{addr}:{port}".format(addr=address, port=port)
        
        self.server: Server = Server()
        self.server.set_endpoint(endpoint)
        self.server.set_server_name(name)
        
        self.ref_nodes = []
    
    def addNamespace(self, namespaceID: str):
        self.server.register_namespace(namespaceID)
    
    def addNamespaces(self, namespaces: list[str]):
        for namespace in namespaces:
            self.addNamespace(namespace) 
        
    def addNode(self, node : UA_Node):
        ref_node = node.addToServer(self.server)
        self.ref_nodes.append(ref_node)
        
    def addNodes(self, nodes : list[UA_Node]):
        for node in nodes:
            node.addToServer(self.server)
    
    def run(self):
        self.server.start()
        
def quick_start(): 
    server = UA_Server("127.0.0.1", 8080, "myOPCUA_server")
    server.addNamespace("2")
    server.enableNodeDataChange(publishInterval=200)
    
    tempNode = UA_Node("2", "TS1", "temperatureNode_1")
    tempNode.addVariable("TS1_test", "value")
    tempNode.addVariable("TS1_Temp", 20)
    
    lightBulb = UA_Node("2", "LB1", "lightBulb_1")
    lightBulb.addVariable("LB1_state", True)
    
    server.addNodes([tempNode, lightBulb])
    server.run()
  
def start_from_json(path):
    lines = open(path, "r").readlines()
    rawData = ''.join(lines)
    config = json.loads(rawData)
    
    server = UA_Server(config["address"], config["port"], config["name"])
    server.addNamespaces(config["namespaces"])
    
    for nodeConfig in config["nodes"]:
        node = UA_Node(nodeConfig)
        server.addNode(node)
        
    server.run()        
  
if __name__ == "__main__":  
    start_from_json("server.config.json")
        
