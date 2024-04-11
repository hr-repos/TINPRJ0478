#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <time.h>
#include "./secret.h"
#include "ServoBarrier.h"
#include "Ultrasonic.h"
#include "StopLight.h"
#define PUBLISH_TOPIC "data/asb1"
#define SUBSCRIBE_TOPIC "asb/1"
#define SUBSCRIBE_TOPIC_VKL "vkl/#"
#define MSG_BUFFER_SIZE (50)

const char *BOT_ID = "1";

const char *ssid = SECRET_SSID;
const char *password = SECRET_PASS;
const char *mqtt_server = MQTT_HOST;

// const char* ntpServer = "pool.ntp.org";
// const long  gmtOffset_sec = 3600;
// const int   daylightOffset_sec = 0;

WiFiClient espClient;
PubSubClient client(espClient);

const uint8_t triggerPin = 35;
const uint8_t echoPin = 34;

Ultrasonic* sonic = new Ultrasonic(triggerPin, echoPin);

unsigned long lastMsg = 0;
char msg[MSG_BUFFER_SIZE];

const uint8_t asbServoPin = 26;
const uint8_t asbLed1Pin = 32;
const uint8_t asbLed2Pin = 33;

const uint8_t greenLedPin = 21;
const uint8_t orangeLedPin = 19;
const uint8_t redLedPin = 18;

auto stopLight = new StopLight(greenLedPin, orangeLedPin, redLedPin);

auto servo = new ServoBarrier(asbServoPin, asbLed1Pin, asbLed2Pin, sonic);

void setup_wifi()
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

void printLocalTime()
{
    struct tm timeInfo;

    if (!getLocalTime(&timeInfo))
    {
        Serial.println("Failed to obtain time");
        return;
    }
    Serial.println(&timeInfo, "%A, %B %d %Y %H:%M:%S");
}

void moveBarrier(bool pos)
{
    pos ? servo->setLocationUp() : servo->setLocationDown();
}

void callback(char *topic, byte *payload, unsigned int length)
{
    char message[length + 1]; // +1 for null terminator

    for (int i = 0; i < length; i++)
    {
        message[i] = (char)payload[i];
    }
    message[length] = '\0';
    // for debugging
    // Serial.printf("Message arrived [%s] %s\n", topic, message);

    // expects X:Y:Z where X, Y, Z is a number
    if (strcmp(topic, SUBSCRIBE_TOPIC) == 0)
    {
        moveBarrier(atoi(&message[0]));

        String returnMessage = "Barrier is " + String((message[0] - '0' == 1) ? "up" : "down");
        client.publish(PUBLISH_TOPIC, returnMessage.c_str());
        Serial.println(returnMessage);
    }

    if(strcmp(topic, "vkl/groen") == 0)
    {
        bool isOn = message[0] - '0';
        stopLight->setLicht(green, isOn);
    }

    if(strcmp(topic, "vkl/oranje") == 0)
    {
        bool isOn = message[0] - '0';
        stopLight->setLicht(orange, isOn);
    }

    if(strcmp(topic, "vkl/rood") == 0)
    {
        bool isOn = message[0] - '0';
        stopLight->setLicht(red, isOn);
    }

    // Print out the message for debugging
    if (strcmp(message, "test") == 0)
    {
        Serial.println("Test message received");
        client.publish(PUBLISH_TOPIC, "Test message received");
    }
}

void reconnect()
{
    // Loop until we're reconnected
    while (!client.connected())
    {
        Serial.print("Attempting MQTT connection...");

        // Attempt to connect
        if (client.connect(BOT_ID, MQTT_USER, MQTT_PASS))
        {
            Serial.println("connected");

            // Once connected, publish an announcement...
            client.publish(PUBLISH_TOPIC, "Bot connected!");

            // ... and resubscribe
            client.subscribe(SUBSCRIBE_TOPIC);

            client.subscribe(SUBSCRIBE_TOPIC_VKL);
        }
        else
        {
            Serial.print("failed, rc=");
            Serial.print(client.state());
            Serial.println(" try again in 5 seconds");
            // Wait 5 seconds before retrying
            delay(5000);
        }
    }
}

void setup()
{
    pinMode(BUILTIN_LED, OUTPUT); // Initialize the BUILTIN_LED pin as an output
    Serial.begin(115200);

    setup_wifi();

    client.setServer(mqtt_server, MQTT_PORT);
    client.setCallback(callback);
    servo->setLocationUp();
}

void loop()
{
    if (!client.connected())
    {
        delay(2000);
        reconnect();
    }
    client.loop();
    servo->callback();
}
