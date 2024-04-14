using OpcLabs.EasyOpc.UA.OperationModel;
using OpcLabs.EasyOpc.UA;

namespace opcuaTestClient
{
    public class UA_Client
    {
        public EasyUAClient Client { get; } = new();

        public object? TryGetNodeValue(UA_Node node, string ValueName)
        {
            return node.TryGetNodeValue(ValueName, Client);
        }

        public bool TrySetNodeValue(UA_Node node, string ValueName, object value)
        {
            return node.TrySetNodeValue(ValueName, value, Client);
        }
    }
}
