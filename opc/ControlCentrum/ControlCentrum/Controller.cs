using ControlCentrum.OPCUA;
using ControlCentrum.Mqtt;

namespace ControlCentrum
{
    public class Controller
    {
        public EasyMqtt Mqtt { get; set; } 
        public UA_Handler Opcua { get; set; } 

        public Controller()
        {
            Mqtt = new(MqttHandler);
            Opcua = new(OPCUA_Handler);
        }
        
        public async Task Run() 
        {
            await Console.Out.WriteLineAsync("starting controller...");

            await Mqtt.Connect();
            await Opcua.Start();

            while (true)
                await Task.Delay(100000);
        }

        private async Task OPCUA_Handler(UA_Variable variable, OpcuaValue value)
        {
            try
            {
                switch (variable.Name)
                {
                    case "mode":
                        await Mqtt.Publish("topic/topic", "mode: " + value);
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"!!<error> message: {ex.Message}!!");
            }

            await Console.Out.WriteLineAsync("\n================================\n");
        }

        private async Task MqttHandler(MqttPayload payload)
        {

            string topic = payload.Topic;
            string message = payload.Message;

            await Console.Out.WriteLineAsync($"topic: {topic}, message: {message}");

            try
            {
                switch (topic)
                {
                    case "mode":
                        (UA_Variable variable, UA_Node node) = GetVariableAndNode(topic);
                        if (!await node.TrySetNodeValue(topic, message, Opcua.Client))
                            Console.Out.WriteLine($"!!<error> node: {node.NodeID.Split('\"')[1]} variable: {variable.Name} no value set!!");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"!!<error> message: {ex.Message}!!");
            }

            await Console.Out.WriteLineAsync("\n================================\n");
        }

        private (UA_Variable, UA_Node) GetVariableAndNode(string topic)
        {
            (UA_Variable? variable, UA_Node? node) = Opcua.TryGetNodeAndVariable_FromName(topic);

            if (variable == null || node == null)
                throw new ArgumentNullException("name not found in a opcua client");

            return (variable, node);
        }
    }
}
