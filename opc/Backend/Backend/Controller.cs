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
        
        public void Run() 
        {
            Console.WriteLine("starting backend...");
            while (true) ;
        }

        private async Task OPCUA_Handler(UA_Variable variable, OpcuaValue value)
        {
            switch (variable.Name)
            {
                case "mode":
                    await Console.Out.WriteLineAsync($"<OPCUA>mode: {value}");
                    await Mqtt.Publish("topic/topic", "mode: " + value.ToString());
                    break;
            }

            await Console.Out.WriteLineAsync("================================");
            return;
        }
        private async Task MqttHandler(MqttPayload payload)
        {
            string topic = payload.Topic;
            string message = payload.Message;

            Console.WriteLine(string.Format("topic: {0}, message: {1}", topic, message ?? "NULL"));

            try 
            {
                switch (topic)
                {
                    case "mode":
                        await Console.Out.WriteLineAsync($"<MQTT>mode: {message}");

                        (UA_Variable variable, UA_Node node) = GetVariableAndNode(topic);
                        if(!node.TrySetNodeValue(topic, message, Opcua.Client))
                            await Console.Out.WriteLineAsync("not bosp;erogufiaodiuf");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            await Console.Out.WriteLineAsync("================================");
            return;
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
