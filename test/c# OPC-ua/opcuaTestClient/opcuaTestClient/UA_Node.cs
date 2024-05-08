using Opc.Ua;
using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.AddressSpace;
using OpcLabs.EasyOpc.UA.AddressSpace.Standard;
using OpcLabs.EasyOpc.UA.OperationModel;
using System.Drawing;
using System.Text;

namespace opcuaTestClient
{
    public class UA_Variable
    {
        public UA_Variable(string name, string varID)
        {
            Name = name;
            VarID = varID;
        }

        public string Name { get; set; }
        public string VarID { get; set; }
        public object? Value { get; set; }
    }

    public class UA_Node
    {
        public UAEndpointDescriptor Endpoint { get; set; }
        public string NodeDescriptor { get; private set; }
        public string NamespaceID { get; set; }

        public Dictionary<string/*Name*/, UA_Variable> Values { get; set; } = new();

        public object this[string name]
        {

            get
            { 
                return Values[name];
            }

            set
            {
                Values[name] = new(name, MakeUA_ID(NamespaceID, (string)value));
            }
        }

        public UA_Node(UAEndpointDescriptor endpoint, string namespaceID, string nodeID)
        {
            Endpoint = endpoint;
            NamespaceID = namespaceID;
            NodeDescriptor = MakeUA_ID(namespaceID, nodeID);
        }

        public void DoEvent(object sender, EasyUADataChangeNotificationEventArgs args)
        {
            Console.WriteLine(args.AttributeData.Value);

            if (args.Succeeded)
            {
                string varID = args.Arguments.NodeDescriptor.NodeId.ToString();
                string? variableName = TryGetVariableFromVarID(varID)?.Name;

                if(variableName == null)
                {
                    Console.WriteLine("NodeDescriptor: "+ varID + " not recognized by this my client");
                    return;
                }

                Console.WriteLine("server heeft de variable: \"{0}\" veranderd naar: {1}", variableName, args.AttributeData.Value);

                Values[variableName].Value = args.AttributeData.Value;
            }
            else
            {
                Console.WriteLine("error: {0}", args.ErrorMessage);
            }
        }

        public void SubDataChange(EasyUAClient client)
        {
            foreach((_, UA_Variable variable) in Values) 
            {
                client.SubscribeDataChange(Endpoint, variable.VarID, 200, DoEvent);
            }

        }


        public List<UA_Variable> UpdateAllVariables(EasyUAClient client)
        {
            List<UA_Variable> variables = new();

            foreach((string valueName, UA_Variable variable) in Values) 
            {
                object? Value = TryGetNodeValue(valueName, client);
                variable.Value = Value;
                variables.Add(variable);
            }

            return variables;
        }

        public object? TryGetNodeValue(string ValueName, EasyUAClient client)
        {
            object? value = null;

            try
            {
                UA_Variable variable = Values[ValueName];
                value = client.ReadValue(Endpoint, variable.VarID);
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
                UA_Variable variable = Values[ValueName];

                client.WriteValue(Endpoint, variable.VarID, value);
                variable.Value = value;
            }
            catch (UAException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private UA_Variable? TryGetVariableFromVarID(string varID)
        {
            return Values.Where(pair => pair.Value.VarID == varID).Select(pair => pair.Value).FirstOrDefault();
        }

        public string? TryGetValueNameFromID(string ID)
        {
            return Values.FirstOrDefault(pair => pair.Value.VarID == ID).Key;
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
