using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace testMqtt.Mqtt
{
    public class MqttPayload
    {
        public MqttPayload(string topic, string message) 
        {
            Topic = topic;
            Message = message;
        }

        public string Topic { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class MqttConfig
    {
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }

        public List<string> Subscribe_topics { get; set; } = new();
        public List<MqttPayload> Publish_topics { get; set; } = new();
    }
}
