from opcua import Server

ENDPOINT = "opc.tcp://127.0.0.1:8080"

class OPC_UA_Server():
    def __init__(self):
        self.server = Server()
        
        self.server.set_endpoint(ENDPOINT)
        self.server.set_server_name("tunnelServer")