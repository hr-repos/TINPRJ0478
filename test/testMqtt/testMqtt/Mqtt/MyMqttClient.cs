using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using System.Text.Json;
using MQTTnet.Server;

namespace testMqtt.Mqtt
{
    public class MyMqttClient
    {
        readonly IMqttClient mqttClient;
        readonly Func<MqttApplicationMessageReceivedEventArgs, Task> messageHandler;

        public MyMqttClient(Func<MqttApplicationMessageReceivedEventArgs, Task> messageHandler)
        {
            this.messageHandler = messageHandler;
            mqttClient = new MqttFactory().CreateMqttClient();
        }

        public async Task<bool> Connect(string configPath)
        {
            StreamReader sr = new(configPath);
            string jsonStr = sr.ReadToEnd();
            
            MqttConfig? json = JsonSerializer.Deserialize<MqttConfig>(jsonStr, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            //MqttConfig? json = new()
            //{
            //    Host = "mq.nl.eu.org",
            //    Port = 8883,
            //    Username = "connectedsystems",
            //    Password = "tincos",
            //    Subscribe_topics = new List<string>()
            //    {
            //        "topic/topic"
            //    },
            //    Publish_topics = new List<PublishInfo>()
            //    {
            //        new("topic/topic", "this is a test")
            //    }

            //};


            if (json == null) 
            {
                //!! log
                return false;
            }

            MqttClientOptions options = new MqttClientOptionsBuilder()
                .WithTcpServer(json.Host, json.Port)
                .WithCredentials(json.Username, json.Password)
                .WithCleanSession()
                .Build();

            MqttClientConnectResult connectResult = await mqttClient.ConnectAsync(options);

            if(!IsConnectionSuccess(connectResult))
            {
                //!! log
                return false;
            }

            try 
            {
                await SubscribeTopics(json.Subscribe_topics);
                //await PublishTopics(json.Publish_topics);
            }
            catch (Exception ex) 
            {
                //!! log ex
                return false;
            }

            //!! log success
            return true;
        }

        public async Task<bool> Connect()
        {
            return await Connect(@"Mqtt\Mqtt.config.json");
        }

        public async Task Publish(string topic, string message)
        {
            await Publish(new(topic, message));
        }

        public async Task Subscribe(string topic)
        {
            await mqttClient.SubscribeAsync(topic);
        }

        private async Task SubscribeTopics(List<string> topics)
        {
            foreach(string topic in topics)
            {
                await mqttClient.SubscribeAsync(topic);
            }

            mqttClient.ApplicationMessageReceivedAsync += messageHandler;
        }

        private async Task PublishTopics(List<PublishInfo> publishes) 
        {
            foreach (PublishInfo publish in publishes)
            {
                await Publish(publish);
            }
        }

        private async Task Publish(PublishInfo publish)
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(publish.Topic)
                .WithPayload(publish.Message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);
        }

        private static bool IsConnectionSuccess(MqttClientConnectResult connectResult) 
        {
            return connectResult.ResultCode != MqttClientConnectResultCode.Success;
        }
    }
}
