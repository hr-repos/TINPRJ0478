global using OpcuaValue = System.Object;

using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;

namespace ControlCentrum.OPCUA
{

    public class UA_Variable
    {
        public UA_Variable(string name, string varID)
        {
            Name = name;
            VarID = varID;
        }

        public UA_Variable(string name, string varID, bool readOnly)
        {
            Name = name;
            VarID = varID;
            ReadOnly = readOnly;
        }

        public string Name { get; set; }
        public string VarID { get; set; }
        public OpcuaValue? Value { get; set; }

        public bool ReadOnly { get; set; } = false;
    }

    public class UA_Node
    {
        public UAEndpointDescriptor Endpoint { get; set; }
        public string NodeID { get; private set; }
        public string Namespace { get; set; }
        public Dictionary<string/*Name*/, UA_Variable> Values { get; set; } = new();

        public UA_Node(UAEndpointDescriptor endpoint, string namespaceID, string nodeID)
        {
            Endpoint = endpoint;
            Namespace = namespaceID;
            NodeID = MakeUA_ID(namespaceID, nodeID);
        }

        public void SubscribeDataChange(EasyUAClient client, int interval = 200)
        {
            foreach ((_, UA_Variable variable) in Values)
            {
                client.SubscribeDataChange(Endpoint, variable.VarID, interval);
            }

        }


        public async Task<List<UA_Variable>> UpdateAllVariables(EasyUAClient client)
        {
            List<UA_Variable> variables = new();

            foreach ((string valueName, UA_Variable variable) in Values)
            {
                OpcuaValue? Value = await TryGetNodeValue(valueName, client);
                variable.Value = Value;
                variables.Add(variable);
            }

            return variables;
        }

        public async Task<OpcuaValue?> TryGetNodeValue(string ValueName, EasyUAClient client)
        {
            OpcuaValue? value;
            UA_Variable? variable = TryGetVariable(ValueName);
            if (variable == null)
            {
                await Console.Out.WriteLineAsync($"!!<error> variableName: {ValueName} not recognized by this my client!!");
                return false;
            }

            try
            {
                variable = Values[ValueName];
                value = client.ReadValue(Endpoint, variable.VarID);
            }
            catch (UAException ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return null;
            }

            return value;
        }

        public async Task<bool> TrySetNodeValue(string ValueName, OpcuaValue value, EasyUAClient client)
        {
            UA_Variable? variable = TryGetVariable(ValueName);
            if(variable == null)
            {
                await Console.Out.WriteLineAsync($"!!<error> variableName: {ValueName} not recognized by this my client!!");
                return false;
            }

            if(variable.ReadOnly == true)
            {
                await Console.Out.WriteLineAsync($"!!<ERROR> cant set: {ValueName} server has set this variable to readonly!!");
                return false;
            }

            try
            {
                variable = Values[ValueName];

                client.WriteValue(Endpoint, variable.VarID, value);
                variable.Value = value;
            }
            catch (UAException ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }

            return true;
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

        public void AddVariable(string name, string id, bool readOnly = false)
        {
            string varDescriptor = MakeUA_ID(Namespace, id);

            Values[name] = new(name, varDescriptor, readOnly);
        }

        public UA_Variable? TryGetVariable(string name)
        {
            if (Values.ContainsKey(name))
                return Values[name];

            return null;
        }
    }
}
