using MQTTnet.Client;
using testMqtt.Mqtt;

MyMqttClient client = new(MessageHandler);

await client.Connect(/*reads default conf ( "Mqtt/Mqtt.config.json" )*/);

//await client.Connect("here path of other config.json");

static Task MessageHandler(MqttApplicationMessageReceivedEventArgs args)
{
    string topic = args.ApplicationMessage.Topic;
    string? message = args.ApplicationMessage.PayloadSegment.ToString();

    if (message == null)
        return Task.CompletedTask;

    Console.WriteLine(string.Format("topic: {0}, message: {1}", topic, message ?? "NULL"));

    /*
     
    code here to handle message
     
     */

    return Task.CompletedTask;
}

await client.Subscribe("test/test");

await client.Publish("test/test", "this is a test");