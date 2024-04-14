using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;

namespace opcuaTestClient
{
    public class UA_Node
    {
        public UAEndpointDescriptor Endpoint { get; set; }
        public string NodeDescriptor { get; set; }

        public Dictionary<string/*Name*/, string/*ID*/> values { get; set; } = new();

        public string this[string name] 
        {
            get
            {
                return values[name];
            }

            set
            {
                values[name] = value;
            } 
        }

        public UA_Node(UAEndpointDescriptor endpoint, string nodeDescriptor)
        {
            Endpoint = endpoint;
            NodeDescriptor = nodeDescriptor;
        }

        public UA_Node(UAEndpointDescriptor endpoint, string nodeDescriptor, Dictionary<string/*Name*/, string/*ID*/> values)
        {
            Endpoint = endpoint;
            NodeDescriptor = nodeDescriptor;

            foreach (string name in values.Keys)
            {
                this.values[name] = values[name];
            }
        }

        public UA_Node(UAEndpointDescriptor endpoint, string nodeDescriptor, List<(string name, string ID)> values) 
        {
            Endpoint = endpoint;
            NodeDescriptor = nodeDescriptor;

            foreach ((string name, string ID) in values) 
            {
                this.values[name] = ID;
            }
        }

        public object? TryGetNodeValue(string ValueName, EasyUAClient client)
        {
            object? value = null;

            try
            {
                value = client.ReadValue(Endpoint, values[ValueName]);
            }
            catch (UAException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }

        public bool TrySetNodeValue(string ValueName, object value, EasyUAClient client)
        {
            try
            {
                client.WriteValue(Endpoint, values[ValueName], value);
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
            return values.FirstOrDefault(pair => pair.Value == ID).Key;
        }

    }
}
