using OpcLabs.EasyOpc.UA;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8080";
UA_Client client = new();


//-------------------makeNode---------------------------------------------


UA_Node tempNode = new
(
    endpoint:    endpoint,
    namespaceID: "2",
    nodeID:      "TS1"
);

tempNode["valueName"] = "ValueID";
tempNode["temperature"] = "TS1_Temp";


//-------------------getValue(can be null)-----------------------------------


string? value = client.TryGetNodeValue(tempNode, "valueName")?.ToString();
string? temp = client.TryGetNodeValue(tempNode, "temperature")?.ToString();

Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));


//-------------------setValue(false if set failed)---------------------------


client.TrySetNodeValue(tempNode, "temperature", 32);
