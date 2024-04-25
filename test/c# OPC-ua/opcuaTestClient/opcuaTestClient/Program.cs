using OpcLabs.EasyOpc.UA;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8080";


//-------------------makeNode---------------------------------------------


UA_Node tempNode = new
(
    endpoint:    endpoint,
    namespaceID: "2",
    nodeID:      "TS1"
);

tempNode["valueName"] = "ValueID";
tempNode["temperature"] = "TS1_Temp";

tempNode.SubDataChange();


//-------------------getValue(can be null)-----------------------------------


string? value = tempNode.TryGetNodeValue("valueName")?.ToString();
string? temp  = tempNode.TryGetNodeValue("temperature")?.ToString();

Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));


//-------------------setValue(false if set failed)---------------------------


tempNode.TrySetNodeValue("temperature", 32);
