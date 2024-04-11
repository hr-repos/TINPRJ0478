using OpcLabs.EasyOpc;
using OpcLabs.EasyOpc.AlarmsAndEvents;
using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;

EasyUAClient client = new();
UAEndpointDescriptor endpoint = "opc.tcp://127.0.0.1:8080";
string nodeDescriptor_TempSensor = "ns=2;s=TS1";


var item = client.SubscribeEvent(endpoint, nodeDescriptor_TempSensor, 200);
Console.WriteLine(item);