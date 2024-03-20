using OpcLabs.EasyOpc.UA;

UAEndpointDescriptor endpoint = "opc.tcp://opcua.demo-this.com:51210/UA/SampleServer";
UANodeDescriptor node = "nsu=http://test.org/UA/Data/;i=10853";

EasyUAClient.SharedInstance.SubscribeEvent(endpoint, node, 200);