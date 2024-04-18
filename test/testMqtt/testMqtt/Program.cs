using MQTTnet.Client;
using testMqtt.Mqtt;

MyMqttClient client = new(MessageHandler);

static Task MessageHandler(MqttApplicationMessageReceivedEventArgs args)
{
    string topic = args.ApplicationMessage.Topic;
    string? message = args.ApplicationMessage.PayloadSegment.ToString();

    Console.WriteLine(string.Format("topic: {0}, message: {1}", topic, message ?? "NULL"));

    /*
     
    code here to handle message
     
     */

    return Task.CompletedTask;
}

await client.Publish("test/test", "this is a test");