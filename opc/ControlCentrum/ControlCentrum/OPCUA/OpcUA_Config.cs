using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlCentrum.OPCUA
{
    public class ConfigVariable
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool ReadOnly { get; set; } = false;
    }

    public class ConfigNode
    {
        public string Name { get; set; } = string.Empty;

        public string NodeID { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;

        public List<ConfigVariable> Variables { get; set; } = new();
    }
    public class OpcUA_Config
    {
        public string Address { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8080;
        public List<ConfigNode> Nodes { get; set; } = new();

    }
}
