using Opc.Ua.Export;
using OpcLabs.BaseLib.Annotations;
using OpcLabs.EasyOpc.UA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opcuaTestClient
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }

    public class UA_client
    {
        public EasyUAClient Client { get; set; } = new();
        public List<UA_Node> UA_Nodes { get; set; } = new();
        public List<List<UA_Variable>> PrevValues { get; set; } = new();
        public int DataChangeCheckInterval { get; set; }

        public UA_client(int dataChangeCheckInterval)
        {
            DataChangeCheckInterval = dataChangeCheckInterval;
        }

        public void AddNode(UA_Node node)
        {
            UA_Nodes.Add(node);
            PrevValues.Add(new());
        }

        public void AddNodes(List<UA_Node> nodes) 
        {
            foreach (UA_Node node in nodes) 
                AddNode(node);
        }

        public void Run()
        {
            foreach((UA_Node node, int index) in UA_Nodes.WithIndex())
                GetAllValues(node, index);

            while (true)
            {
                Thread.Sleep(DataChangeCheckInterval);
                CheckForNewValues();
                Console.WriteLine("=====================================");
            }

        }

        private void CheckForNewValues() 
        { 
            foreach((UA_Node node, int index) in UA_Nodes.WithIndex())
            {
                List<UA_Variable> oldValues;

                oldValues = PrevValues[index];

                var newValues = node.UpdateAllVariables(Client);

                var changesValues = newValues.Where((variable, index) => variable.Value != newValues).ToList();
                changesValues.ForEach(value => { Console.WriteLine("Name: {0}, \tValue: {1}", value.Name, value.Value); });

                newValues = oldValues;
            }
        }

        private void GetAllValues(UA_Node node, int nodeIndex)
        {
            var vars = node.Values.Values;
            foreach ((UA_Variable variable, int index) in vars.WithIndex())
            {
                variable.Value = node.TryGetNodeValue(variable.Name, Client);
                try
                {
                    PrevValues[nodeIndex][index] = variable;
                }
                catch 
                {

                    PrevValues[nodeIndex].Add(variable);
                }

            }
        }
    }
}
