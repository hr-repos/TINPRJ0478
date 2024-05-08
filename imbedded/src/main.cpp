#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <time.h>
#include "./secret.h"
#include "ServoBarrier.h"
#include "Ultrasonic.h"
#define MESSAGE_TOPIC "asb/1/messages"
#define ASB_FORCED_TOPIC "asb/1/forced"
#define ASB_STANDARD_TOPIC "asb/1/standard"
#define LANE_WIDTH_CM 10

const char* BOT_ID = "1";

const char* ssid = SECRET_SSID;
const char* password = SECRET_PASS;
const char* mqtt_server = MQTT_HOST;

// const char* ntpServer = "pool.ntp.org";
// const long  gmtOffset_sec = 3600;
// const int   daylightOffset_sec = 0;

char msg[MSG_BUFFER_SIZE];
WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;
#define MSG_BUFFER_SIZE	(50)

uint8_t asbServoPin = 26;
uint8_t asbLedPin1 = LED_BUILTIN;
uint8_t asbLedPin2 = LED_BUILTIN;
// ServoBarrier servo(asbServoPin, asbLedPin);

auto ultrasoon = new Ultrasonic(13, 12);
barrierData asbConfig = {asbServoPin, asbLedPin1, asbLedPin2, LANE_WIDTH_CM, ultrasoon};
auto servo = new ServoBarrier(asbConfig);

void setup_wifi() {
    delay(10);
    // We start by connecting to a WiFi network
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(ssid);

    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }

    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}

void printLocalTime() {
  struct tm timeinfo;
  if(!getLocalTime(&timeinfo)){
    Serial.println("Failed to obtain time");
    return;
  }
  Serial.println(&timeinfo, "%A, %B %d %Y %H:%M:%S");
}

void moveBarrier(bool pos) {
    pos ? servo->setLocationUp() : servo->setRequestedPositionDown();
}


void callback(char* topic, byte* payload, unsigned int length) {
    char message[length + 1]; // +1 for null terminator
    for (int i = 0; i < length; i++) {
        message[i] = (char)payload[i];
    }
    message[length] = '\0';
    // for debugging
    // Serial.printf("Message arrived [%s] %s\n", topic, message);

    if (strcmp(topic, ASB_STANDARD_TOPIC) == 0) {
        if (servo->objectDetected()) {
            client.publish(MESSAGE_TOPIC, "5");
            Serial.println("Object detected in lane, barrier cannot be moved.");
            return;
        }
        
        moveBarrier(atoi(&message[0]));

        String returnMessage = "Barrier is " + String((message[0]- '0' == 1) ? "up" : "down");
        client.publish(MESSAGE_TOPIC, returnMessage.c_str());
        Serial.println(returnMessage);
    }

    if (strcmp(topic, ASB_FORCED_TOPIC) == 0) {
        moveBarrier(atoi(&message[0]));

        String returnMessage = "Barrier is " + String((message[0]- '0' == 1) ? "up" : "down");
        client.publish(MESSAGE_TOPIC, returnMessage.c_str());
        Serial.println(returnMessage);
    }

    // Print out the message for debugging
    if (strcmp(message, "test") == 0) {
        Serial.println("Test message received");
        client.publish(MESSAGE_TOPIC, "Test message received");
    }
}

void reconnect() {
    // Loop until we're reconnected
    while (!client.connected()) {
        Serial.print("Attempting MQTT connection...");

        // Attempt to connect
        if (client.connect(BOT_ID, MQTT_USER, MQTT_PASS)) {
            Serial.println("connected");

            // Once connected, publish an announcement...
            client.publish(MESSAGE_TOPIC, "Bot connected!");

            // ... and resubscribe
            client.subscribe(ASB_FORCED_TOPIC);
            client.subscribe(ASB_STANDARD_TOPIC);
        } else {
            Serial.print("failed, rc=");
            Serial.print(client.state());
            Serial.println(" try again in 5 seconds");
            // Wait 5 seconds before retrying
            delay(5000);
        }
    }
}

void setup() {
    pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
    Serial.begin(115200);

    setup_wifi();

    client.setServer(mqtt_server, MQTT_PORT);
    client.setCallback(callback);
    servo->setLocationUp();
}

void loop() {
    if (!client.connected()) {
        delay(2000);
        reconnect();
    }
    client.loop();
    servo->callback();


}
