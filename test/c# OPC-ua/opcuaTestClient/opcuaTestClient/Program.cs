using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.AddressSpace.Extensions;
using OpcLabs.EasyOpc.UA.Extensions;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://145.24.223.210:8100";
EasyUAClient client = new();

//-------------------makeNode---------------------------------------------
UA_Node tempNode = new
(
    endpoint:    endpoint,
    namespaceID: "2",
    nodeID:      "TS1"
);

tempNode["valueName"] = "TS1_test";
tempNode["temperature"] = "TS1_Temp";

tempNode.SubDataChange(client);

while (true) ;

//-------------------getValue(can be null)-----------------------------------


//string? value = tempNode.TryGetNodeValue("valueName", _client)?.ToString();
//string? temp = tempNode.TryGetNodeValue("temperature", _client)?.ToString();

//Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));

//Thread.Sleep(1000);

//-------------------setValue(false if set failed)---------------------------


//tempNode.TrySetNodeValue("temperature", 32, _client);
