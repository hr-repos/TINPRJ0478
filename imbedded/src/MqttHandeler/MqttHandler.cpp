#include "MqttHandler.h"

static char *botID = (char *)"1";
static Mqtt *MqttClient = new Mqtt(SECRET_SSID, SECRET_PASS, botID, MQTT_USER, MQTT_PASS);

const uint8_t triggerPin = 35;
const uint8_t echoPin = 34;

Ultrasonic *sonic = new Ultrasonic(triggerPin, echoPin);

const uint8_t asbServoPin = 26;
const uint8_t asbLed1Pin = 32;
const uint8_t asbLed2Pin = 33;

const uint8_t greenLedPin = 21;
const uint8_t orangeLedPin = 19;
const uint8_t redLedPin = 18;

auto stopLight = new StopLight(greenLedPin, orangeLedPin, redLedPin);

auto servo = new ServoBarrier(asbServoPin, asbLed1Pin, asbLed2Pin, sonic);

int charToInt(char ch)
{
    return ch - '0';
}

int charToInt(char* ch)
{
    return (*ch) - '0';
}

void publishAndLogMessage(String message, PubSubClient* client)
{
    client->publish(PUBLISH_TOPIC, message.c_str());
    Serial.println(message);
}

void moveBarrier(bool pos)
{
    pos ? servo->setLocationUp() : servo->setLocationDown();
}

void pubSub(PubSubClient *client)
{
    Serial.println("connected");

    // Once connected, publish an announcement...
    client->publish(PUBLISH_TOPIC, "Bot connected!");

    //... and resubscribe
    client->subscribe(SUBSCRIBE_TOPIC);
    client->subscribe(SUBSCRIBE_TOPIC_VKL);
}

void messageHandler(PubSubClient *client, char *topic, char *message, unsigned int length)
{
    // expects X:Y:Z where X, Y, Z is a number

    if (strcmp(topic, SUBSCRIBE_TOPIC) == 0)
    {
        bool isUp = charToInt(&message[0]);
        moveBarrier(isUp);

        String returnMessage = "Barrier is " + String((isUp) ? "up" : "down");
        publishAndLogMessage(returnMessage, client);
    }

    if (strcmp(topic, "vkl/groen") == 0)
    {
        bool isOn = charToInt(message[0]);
        stopLight->setLicht(green, isOn);

        String returnMessage = "greenLed is " + String((isOn) ? "on" : "off");
        publishAndLogMessage(returnMessage, client);
    }

    if (strcmp(topic, "vkl/oranje") == 0)
    {
        bool isOn = charToInt(message[0]);
        stopLight->setLicht(orange, isOn);

        String returnMessage = "orangeLed is " + String((isOn) ? "on" : "off");
        publishAndLogMessage(returnMessage, client);
    }

    if (strcmp(topic, "vkl/rood") == 0)
    {
        bool isOn = charToInt(message[0]);
        stopLight->setLicht(red, isOn);

        String returnMessage = "redLed is " + String((isOn) ? "on" : "off");
        publishAndLogMessage(returnMessage, client);
    }

    // Print out the message for debugging
    if (strcmp(message, "test") == 0)
    {
        Serial.println("Test message received");
        client->publish(PUBLISH_TOPIC, "Test message received");
    }
}

void MqttSetup()
{
    servo->setLocationUp();

    MqttClient->connectMqtt(MQTT_HOST, MQTT_PORT, pubSub, messageHandler);
}

void MqttLoop()
{
    auto client = MqttClient->getClient();

    if (!client->connected())
    {
        delay(2000);
        MqttClient->reconnect();
    }

    client->loop();
    servo->callback();
}
