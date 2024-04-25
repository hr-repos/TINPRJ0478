using Opc.Ua;
using Opc.Ua.Gds;
using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;

namespace opcuaTestClient
{
    public class UA_Node
    {
        static public EasyUAClient Client { get; } = new();
        public UAEndpointDescriptor Endpoint { get; set; }
        public string NodeDescriptor { get; private set; }
        public string NamespaceID { get; set; }

        public Dictionary<string/*Name*/, string/*ID*/> Values { get; set; } = new();

        public string this[string name] 
        {
            get
            {
                return Values[name];
            }

            set
            {
                Values[name] = MakeUA_ID(NamespaceID, value);
            } 
        }

        public UA_Node(UAEndpointDescriptor endpoint, string namespaceID, string nodeID)
        {
            Endpoint = endpoint;
            NamespaceID = namespaceID;
            NodeDescriptor = MakeUA_ID(namespaceID, nodeID);
        }

        public UA_Node(UAEndpointDescriptor endpoint, string namespaceID, string nodeID, Dictionary<string/*Name*/, string/*ID*/> values)
        {
            Endpoint = endpoint;
            NamespaceID = namespaceID;
            NodeDescriptor = MakeUA_ID(namespaceID, nodeID);

            foreach (string name in values.Keys)
            {
                this.Values[name] = values[name];
            }
        }

        public UA_Node(UAEndpointDescriptor endpoint, string namespaceID, string nodeID, List<(string name, string ID)> values) 
        {
            Endpoint = endpoint;
            NamespaceID = namespaceID;
            NodeDescriptor = MakeUA_ID(namespaceID, nodeID);

            foreach ((string name, string ID) in values) 
            {
                this.Values[name] = ID;
            }
        }

        public void SubDataChange() 
        {
            object? changedValue;

            Client.SubscribeDataChange(Endpoint, NodeDescriptor, 200, 
                (sender, args) =>
                {
                    if(args.Succeeded) 
                    {
                        string? variableName = args.Arguments.NodeDescriptor.AliasName ?? args.Arguments.NodeDescriptor.ToString();

                        Console.WriteLine("server heeft de variable \"{0}\" veranderd naar: {1}", args.Arguments.NodeDescriptor, args.AttributeData.Value);
                        Console.WriteLine(args.Arguments.NodeDescriptor.AliasName);
                        changedValue = args.AttributeData.Value;
                    }
                    else
                    {
                        Console.WriteLine("error: {0}", args.ErrorMessage);
                    }
                }
            );
        }

        public object? TryGetNodeValue(string ValueName)
        {
            object? value = null;

            try
            {
                value = Client.ReadValue(Endpoint, Values[ValueName]);
            }
            catch (UAException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }

        public bool TrySetNodeValue(string ValueName, object value)
        {
            try
            {
                Client.WriteValue(Endpoint, Values[ValueName], value);
            }
            catch (UAException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public string? TryGetValueNameFromID(string ID)
        {
            return Values.FirstOrDefault(pair => pair.Value == ID).Key;
        }

        public string[] GetValueNames() 
        {
            return Values.Keys.ToArray();
        }

        private static string MakeUA_ID(string namespaceID, string objID)
        {
            return string.Format("ns={0};s=\"{1}\"", namespaceID, objID);
        }
    }
}
