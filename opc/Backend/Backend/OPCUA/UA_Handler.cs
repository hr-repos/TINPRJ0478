using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.Generic;
using System;
using OpcLabs.EasyOpc.UA.OperationModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using testMqtt.Mqtt;

namespace Backend.OPCUA
{
    public class UA_Handler
    {
        public EasyUAClient Client { get; set; } = new();
        public Func<UA_Variable, OpcuaValue, Task> Handler { get; set; }

        public List<UA_Node> Nodes { get; set; } = new();
        public Dictionary<string/*nodeID*/, string/*name*/> Variables { get; set; } = new();

        public UA_Handler(Func<UA_Variable, OpcuaValue, Task> handler, string configPath = "OPCUA/opcuaClient.config.json")
        {
            Handler = handler;

            OpcUA_Config? config = GetJsonConfig(configPath);
            if (config == null)
                return;

            Client.DataChangeNotification += DataChangeHandler;

            SetupClient(config);
        }

        private static OpcUA_Config? GetJsonConfig(string path) 
        {
            StreamReader reader = new(path);
            string rawJson = reader.ReadToEnd();

            OpcUA_Config? config = JsonSerializer.Deserialize<OpcUA_Config>(rawJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (config == null)
                Console.WriteLine("!!error failed to load config.json!!");

            return config;
        }

        private void SetupClient(OpcUA_Config config)
        {
            string endpoint = $"opc.tcp://{config.Address}:{config.Port}";

            foreach (var node in config.Nodes)
            {
                UA_Node newNode = new
                (
                    endpoint,
                    node.Namespace,
                    node.NodeID
                );

                foreach (var value in node.Variables)
                {
                    string name = value.Name;
                    string id = value.Id;


                    newNode.AddVariable(name, id);

                    string varDescriptor = $"ns={node.Namespace};s=\"{id}\"";
                    Variables[varDescriptor] = name;
                }

                newNode.SubscribeDataChange(Client);
                
                Nodes.Add(newNode);
            }
        }

        private async void DataChangeHandler(object sender, EasyUADataChangeNotificationEventArgs args)
        {
            if (args.Succeeded)
            {
                object value = args.AttributeData.Value;

                string varID = args.Arguments.NodeDescriptor.NodeId.ToString();

                string? variableName = Variables[varID];

                (UA_Variable? variable, _) = TryGetNodeAndVariable_FromName(variableName);

                if (variableName == null || variable == null)
                {
                    Console.WriteLine($"NodeDescriptor: {varID} not recognized by this my client");
                    return;
                }

                Console.WriteLine($"server heeft de variable: \"{variableName}\" veranderd naar: {value}");

                await Handler(variable, value);
            }
            else
            {
                Console.WriteLine($"error: {args.ErrorMessage}");
            }
        }

        public (UA_Variable? variable, UA_Node? node) TryGetNodeAndVariable_FromName(string VariableName)
        {
            UA_Variable? variable;
            foreach(UA_Node node in Nodes)
            {
                variable = node.TryGetVariable(VariableName);

                if(variable != null)
                    return (variable, node);
            }

            return (null, null); 
        }


    }
}
