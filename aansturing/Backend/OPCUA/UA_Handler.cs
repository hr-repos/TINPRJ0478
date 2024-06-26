﻿using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;
using System.Text.Json;
using OpcLabs.EasyOpc.UA.AddressSpace.Standard;

namespace Backend.OPCUA
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }

    public class UA_Handler
    {
        public EasyUAClient Client { get; set; } = new();
        public Func<UA_Variable, OpcuaValue, Task> Handler { private get; set; }

        public List<UA_Node> Nodes { get; set; } = new();
        public Dictionary<string/*nodeID*/, string/*name*/> Variables { get; set; } = new();

        public string[]? NamespaceArray { get; set; }


        public UA_Handler(Func<UA_Variable, OpcuaValue, Task> handler, string configPath = "OPCUA/opcuaClient.config.json")
        {
            Handler = handler;

            OpcUA_Config? config = GetJsonConfig(configPath);
            if (config == null)
                return;

            SetupClient(config);
        }

        public async Task<bool> Start()
        {
            try
            {
                await Console.Out.WriteLineAsync("\nstarting opcua connection...\n");

                Client.DataChangeNotification += DataChangeHandler;

                foreach (var node in Nodes)
                {
                    await node.UpdateAllVariables(Client);
                    node.SubscribeDataChange(Client);
                }

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            return false;
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
            NamespaceArray = (string[])Client.ReadValue(endpoint, UAVariableIds.Server_NamespaceArray);

            foreach (var node in config.Nodes)
            {
                string? namespaceIndex = FindNamespaceIndex(node.Namespace);
                if (namespaceIndex == null)
                {
                    Console.WriteLine($"!!<error> failed to get namespaceIndex of node: {node.Name}!!");
                    continue;
                }

                UA_Node newNode = new
                (
                    endpoint,
                    namespaceIndex,
                    node.NodeID
                );

                foreach (var value in node.Variables)
                {
                    string name = value.Name;
                    string id = value.Id;


                    newNode.AddVariable(name, id);

                    string varDescriptor = $"ns={namespaceIndex};s=\"{id}\"";
                    Variables[varDescriptor] = name;
                }

                Nodes.Add(newNode);
            }
        }

        private async void DataChangeHandler(object sender, EasyUADataChangeNotificationEventArgs args)
        {

            if (!args.Succeeded)
            {
                await Console.Out.WriteLineAsync($"error: {args.ErrorMessage}");
                return;
            }

            object value = args.AttributeData.Value;

            string varID = args.Arguments.NodeDescriptor.NodeId.ToString();

            string? variableName = Variables[varID];

            (UA_Variable? variable, UA_Node? node) = TryGetNodeAndVariable(variableName, varID);

            if (variableName == null || variable == null || node == null)
            {
                await Console.Out.WriteLineAsync($"!!<ERROR> NodeDescriptor: {variableName} not recognized by this my client!!");
                return;
            }

            await Console.Out.WriteLineAsync($"<opcua> server has the node: \"{node.NodeID.Split('\"')[1]}\", variable: \"{variableName}\" changes to: {value}");
            try
            {
                await Handler(variable, value);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"!!<error> messageHandler throws exception: {ex.Message}!!");
            }
        }

        private string? FindNamespaceIndex(string namespaceName)
        {
            if (NamespaceArray == null)
                return null;

            foreach ((string name, int index) in NamespaceArray.WithIndex())
            {
                if (name == namespaceName)
                    return index.ToString();
            }

            return null;
        }

        public (UA_Variable? variable, UA_Node? node) TryGetNodeAndVariable_fromMqtt(string VariableName, string varID)
        {
            UA_Variable? variable;
            foreach (UA_Node node in Nodes)
            {
                variable = node.TryGetVariable(VariableName);

                if (variable == null)
                    continue;
                string? namespaceIndex = FindNamespaceIndex(node.Namespace) ?? "2";
                string varDescriptor = $"ns={namespaceIndex};s=\"{varID}\"";

                if (variable.VarID == varDescriptor)
                    return (variable, node);
            }

            return (null, null);

        }

        public (UA_Variable? variable, UA_Node? node) TryGetNodeAndVariable(string VariableName, string varID)
        {
            UA_Variable? variable;
            foreach (UA_Node node in Nodes)
            {
                variable = node.TryGetVariable(VariableName);

                if (variable == null)
                    continue;

                if (variable.VarID == varID)
                    return (variable, node);
            }

            return (null, null);
        }


    }
}
