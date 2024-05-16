using Backend.OPCUA;
using Backend.Mqtt;

namespace Backend
{
    public class Controller
    {
        public EasyMqtt Mqtt { get; set; }
        public UA_Handler Opcua { get; set; }
        static readonly string COMMANDO = "terugkoppeling";

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

            Mqtt.MessageHandler = MqttHandler;
            Opcua.Handler = OPCUA_Handler;

            while (true)
                await Task.Delay(100000);
        }

        private async Task OPCUA_Handler(UA_Variable variable, OpcuaValue nodeValue)
        {
            try
            {
                (_, UA_Node? node) = Opcua.TryGetNodeAndVariable(variable.Name, variable.VarID);
                if (node == null)
                {
                    await Console.Out.WriteLineAsync($"!!<error> no node found for variable: {variable.Name}!!");
                    return;
                }

                string topic = GetMqttTopicFromUA_Node(node);
                string value = nodeValue?.ToString() ?? "0";

                switch (variable.Name)
                {
                    case "mode":
                        await Mqtt.Publish(topic, value);
                        await Console.Out.WriteLineAsync($"<mqtt> published[\"{topic} : {value}\"]");
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

            string[] topics = topic.Split('/')[0..2];
            string nodeID = string.Join("", topics);


            await Console.Out.WriteLineAsync($"<mqtt>subscribed[\"{topic} : {message}\"], nodeID: {nodeID}");

            UA_Variable variable;
            UA_Node node;

            if (message == "reset")
            {
                foreach (UA_Node currentNode in Opcua.Nodes)
                {
                    UA_Variable? typeVar = currentNode.TryGetVariable("type");
                    if (typeVar == null)
                        continue;

                    string type = (string?)typeVar.Value ?? "";
                    int modeValue = 0;

                    switch (type)
                    {
                        case "vkl":
                            modeValue = 0;
                            break;

                        case "asb":
                            modeValue = 2;
                            break;
                    }

                    await currentNode.TrySetNodeValue("mode", modeValue, Opcua.Client);
                }
                return;
            }

            try
            {
                (variable, node) = GetVariableAndNode("mode", nodeID);

                if (!await node.TrySetNodeValue("mode", message, Opcua.Client))
                    Console.Out.WriteLine($"!!<error> node: {node.NodeID.Split('\"')[1]} variable: {variable.Name} no value set!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"!!<error> message: {ex.Message}!!");
            }

            await Console.Out.WriteLineAsync("\n================================\n");
        }

        private static int OpcuaNumberToInt(object opcuaValue)
        {
            int value;

            if (opcuaValue.GetType() == typeof(Boolean))
                value = ((bool)opcuaValue) ? 1 : 0;
            else
                value = (int)opcuaValue;

            return value;
        }

        private static string GetMqttTopicFromUA_Node(UA_Node node)
        {
            string? nodeID = ((long?)node.Values["id"].Value).ToString();
            string? nodeType = (string?)node.Values["type"].Value;

            if (nodeID == null || nodeType == null)
                return "";

            return $"{nodeType}/{nodeID}/{COMMANDO}";
        }

        private (UA_Variable, UA_Node) GetVariableAndNode(string variableName, string nodeID)
        {
            (UA_Variable? variable, UA_Node? node) = Opcua.TryGetNodeAndVariable_fromMqtt(variableName, $"{nodeID}_{variableName}");

            if (variable == null || node == null)
                throw new Exception($"name not found in a opcua client {$"{nodeID}_{variableName}"}");

            return (variable, node);
        }
    }
}
