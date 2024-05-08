using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.AddressSpace.Extensions;
using OpcLabs.EasyOpc.UA.Extensions;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8080";


//-------------------makeNode---------------------------------------------
EasyUAClient _client = new();

UA_Node tempNode = new
(
    endpoint:    endpoint,
    namespaceID: "2",
    nodeID:      "TS1"
);

tempNode["valueName"] = "TS1_test";
tempNode["temperature"] = "TS1_Temp";

//var notify = _client.BrowseDataNodes(endpoint);
//foreach (var item in notify)
//{
//    Console.WriteLine(item.DisplayName);
//}

//_client.SubscribeDataChange(endpoint, tempNode.NodeDescriptor, 200,
//    (sender, args) =>
//    {
//        if (args.Succeeded)
//        {

//        }
//        else
//        {
//            Console.WriteLine(args.ErrorMessage);
//        }
//    });

UA_client client = new(200);
client.AddNode(tempNode);
client.Run();

//-------------------getValue(can be null)-----------------------------------


//string? value = tempNode.TryGetNodeValue("valueName", _client)?.ToString();
//string? temp = tempNode.TryGetNodeValue("temperature", _client)?.ToString();

//Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));

//Thread.Sleep(1000);

//-------------------setValue(false if set failed)---------------------------


//tempNode.TrySetNodeValue("temperature", 32, _client);
