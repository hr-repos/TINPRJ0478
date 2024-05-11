using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.AddressSpace.Extensions;
using OpcLabs.EasyOpc.UA.Extensions;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8100";
EasyUAClient client = new();

//-------------------makeNode---------------------------------------------

UA_Node vkl1 = new
(
    endpoint: endpoint,
    namespaceID: "verander",
    nodeID: "vlk1"
);



tempNode["id"] = "vlk1_ID"; //nodeID
tempNode["mode"] = "vlk1_mode";

string? value = vkl1.TryGetNodeValue("id", client)?.ToString();

//tempNode.SubDataChange(client);

//while (true) ;

//-------------------getValue(can be null)-----------------------------------


//string? value = tempNode.TryGetNodeValue("valueName", _client)?.ToString();
//string? temp = tempNode.TryGetNodeValue("temperature", _client)?.ToString();

//Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));

//Thread.Sleep(1000);

//-------------------setValue(false if set failed)---------------------------


//tempNode.TrySetNodeValue("temperature", 32, _client);
