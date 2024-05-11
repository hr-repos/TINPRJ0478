using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using System.Text.Json;
using MQTTnet.Server;

namespace testMqtt.Mqtt
{
    public class EasyMqtt
    {
        readonly IMqttClient mqttClient;
        readonly Func<MqttPayload, Task> messageHandler;

        public EasyMqtt(Func<MqttPayload, Task> messageHandler)
        {
            this.messageHandler = messageHandler;
            mqttClient = new MqttFactory().CreateMqttClient();
        }

        public async Task<bool> Connect(string configPath)
        {
            StreamReader sr = new(configPath);
            string jsonStr = sr.ReadToEnd();

            MqttConfig? json = JsonSerializer.Deserialize<MqttConfig>(jsonStr);

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
                await Console.Out.WriteLineAsync(string.Format("!!error!! connection not successful: {0}", connectResult.ReasonString));
                return false;
            }

            await SubscribeTopics(json.Subscribe_topics);
            await PublishTopics(json.Publish_topics);

            //!! log success
            return true;
        }

        public async Task<bool> Connect()
        {
            return await Connect("Mqtt.config.json");
        }

        public async Task Publish(string topic, string message)
        {
            await Publish(new(topic, message));
        }

        public async Task<bool> Subscribe(string topic)
        {
            try
            {
                await mqttClient.SubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(string.Format("!!error!! while subscribing[topic:{0}]: {1}",topic ,ex.Message));
                return false;
            }

            return true;
        }

        private async Task<bool> SubscribeTopics(List<string> topics)
        {
            foreach(string topic in topics)
            {
                bool isSuccess = await Subscribe(topic);

                if (!isSuccess)
                    return false;
            }

            mqttClient.ApplicationMessageReceivedAsync += MessageReceivedEventHandler;

            return true;
        }

        private async Task MessageReceivedEventHandler(MqttApplicationMessageReceivedEventArgs args)
        {
            string topic = args.ApplicationMessage.Topic;
            string? message = args.ApplicationMessage.PayloadSegment.ToString();

            if (message == null)
            {
                await Console.Out.WriteLineAsync("!!error!! message is empty");
                return;
            }

            try
            {
                await messageHandler(new MqttPayload(topic, message));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(string.Format("!!error!! messageHandler throws exception[topic: {0}, message: {1}]: {2}",topic, message, ex.Message));
            }
        }

        private async Task<bool> PublishTopics(List<MqttPayload> publishes) 
        {
            foreach (MqttPayload publish in publishes)
            {
                bool isSuccess = await Publish(publish);

                if (!isSuccess)
                    return false;
            }

            return true;
        }

        private async Task<bool> Publish(MqttPayload publish)
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(publish.Topic)
                .WithPayload(publish.Message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            try
            {
                await mqttClient.PublishAsync(message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(string.Format("error while publishing[topic:{0}, message{1}]: {2}",publish.Topic, publish.Message ,ex.Message));
                return false;
            }

            return true;

        }

        private static bool IsConnectionSuccess(MqttClientConnectResult connectResult) 
        {
            return connectResult.ResultCode != MqttClientConnectResultCode.Success;
        }
    }
}
