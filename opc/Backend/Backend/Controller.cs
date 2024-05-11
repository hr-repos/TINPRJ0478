using Backend.OPCUA;
using testMqtt.Mqtt;

namespace Backend
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

            if (await Mqtt.Connect())
                await Console.Out.WriteLineAsync("mqtt connection SUCCESS");

            if (await Opcua.Start())
                await Console.Out.WriteLineAsync("opcua connection SUCCESS");

            while (true)
                await Task.Delay(100000);
        }

        private async Task OPCUA_Handler(UA_Variable variable, OpcuaValue value)
        {

            switch (variable.Name)
            {
                case "mode":
                    await Console.Out.WriteLineAsync($"<OPCUA>mode: {value}");
                    await Mqtt.Publish("topic/topic", "mode: " + value);
                    break;
            }

            await Console.Out.WriteLineAsync("\n================================\n");
            return;
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
                        await Console.Out.WriteLineAsync($"<MQTT>mode: {message}");

                        (UA_Variable variable, UA_Node node) = GetVariableAndNode(topic);

                        if (!await node.TrySetNodeValue(topic, message, Opcua.Client))
                            Console.Out.WriteLine("!!<error> no value set!!");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            await Console.Out.WriteLineAsync("\n================================\n");
        }

        private (UA_Variable, UA_Node) GetVariableAndNode(string topic)
        {
            (UA_Variable? variable, UA_Node? node) = Opcua.TryGetNodeAndVariable_FromName(topic);

            if (variable == null)
                throw new ArgumentNullException("name not found in a opcua client");

            if (node == null)
                throw new ArgumentNullException("name not found in a opcua client");

            return (variable, node);
        }
    }
}
