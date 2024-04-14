using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;
using opcuaTestClient;

UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8080";

UA_Client client = new();

//-------------------makeNode way 1-----------------------------------

UA_Node tempNode = new(endpoint, "ns=2;s=\"TS1\"");
tempNode["valueName"] = "ns=2;s=\"ValueID\"";
tempNode["temperature"] = "ns=2;s=\"TS1_Temp\"";

//-------------------makeNode way 2-----------------------------------

var lightBulbValues = new List<(string name, string id)>()
{
    ("state", "LB1_State"),
    ("brightness", "LB1_Brightness")
};

UA_Node lightBulb = new(endpoint, "ns=2;s=\"LB1\"", lightBulbValues);

//-------------------getValue(can be null)-----------------------------------

string? value = client.TryGetNodeValue(tempNode, "valueName")?.ToString();
string? temp = client.TryGetNodeValue(tempNode, "temperature")?.ToString();

Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));

//-------------------setValue(false if set failed)-----------------------------------

client.TrySetNodeValue(tempNode, "temperature", 32);

//------------------------------------------------------


value = client.TryGetNodeValue(tempNode, "valueName")?.ToString();
temp = client.TryGetNodeValue(tempNode, "temperature")?.ToString();

Console.WriteLine(string.Format("value: {0}, temp: {1}", value ?? "null", temp ?? "null"));


client.TrySetNodeValue(tempNode, "temperature", 21);
