#include "Mqtt.h"

static void (*globalCallBack)(PubSubClient *client, char *topic, char *message, unsigned int length);
static PubSubClient *globalClient;

void callback(char *topic, uint8_t *payload, unsigned int length)
{
    char message[length + 1]; // +1 for null terminator

    for (int i = 0; i < length; i++)
    {
        message[i] = (char)payload[i];
    }
    message[length] = '\0';

    globalCallBack(globalClient, topic, message, length);
}

//----------------------------------------------------------------

Mqtt::Mqtt(String ssid, String password, String ID, String MqttUser, String MqttPass)
    : ssid(ssid), password(password), BOT_ID(ID), MqttUser(MqttUser), MqttPass(MqttPass)
{
    globalClient = client = new PubSubClient(espClient);

    setupWifi();
}

Mqtt::~Mqtt()
{
    if (client != nullptr)
        delete client;
}

void Mqtt::connectMqtt(String mqttHost, int mqttPort, void (*pubSub)(PubSubClient *client), void (*myCallBack)(PubSubClient *client, char *topic, char *message, unsigned int length))
{
    this->pubSub = pubSub;
    globalCallBack = myCallBack;

    client->setServer(mqttHost.c_str(), mqttPort);
    client->setCallback(callback);
}

void Mqtt::setupWifi()
{
    delay(10);

    // We start by connecting to a WiFi network
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(ssid);

    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }

    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}

void Mqtt::printLocalTime()
{
    struct tm timeInfo;

    if (!getLocalTime(&timeInfo))
    {
        Serial.println("Failed to obtain time");
        return;
    }
    Serial.println(&timeInfo, "%A, %B %d %Y %H:%M:%S");
}

void Mqtt::reconnect()
{
    // Loop until we're reconnected
    while (!client->connected())
    {
        Serial.print("Attempting MQTT connection...");

        // Attempt to connect
        if (client->connect(BOT_ID.c_str(), MqttUser.c_str(), MqttPass.c_str()))
        {
            pubSub(client);
        }
        else
        {
            Serial.print("failed, rc=");
            Serial.print(client->state());
            Serial.println(" try again in 5 seconds");
            // Wait 5 seconds before retrying
            delay(5000);
        }
    }
}

PubSubClient *Mqtt::getClient()
{
    return client;
}