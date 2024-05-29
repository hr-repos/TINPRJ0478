#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <cstdint>
#include <time.h>
#include "./secret.h"
#include "ServoBarrier.h"
#include "StopLight.h"
#include "Timer/Timer.h"
#include "Ultrasonic.h"
// #include "time_functions.cpp"
#define BOT_MESSAGE_TOPIC "bot1/messages"
#define ASB_MESSAGE_TOPIC "asb/1/terugkoppeling"
#define ASB_INCOMING_TOPIC "asb/1/verander"
#define VKL_MESSAGE_TOPIC "vkl/1/terugkoppeling"
#define VKL_INCOMING_TOPIC "vkl/1/verander"
// #define ASB_FORCED_TOPIC "asb/1/forced"
// #define ASB_STANDARD_TOPIC "asb/1/standard"
#define LANE_WIDTH_CM 10

const char* BOT_ID = "asb1";

const char* ssid = SECRET_SSID;
const char* password = SECRET_PASS;
const char* mqtt_server = MQTT_HOST;

enum veranderProtocolASB {
    OPENEN = 0,
    NORMAALSLUITEN = 1,
    GEFORCEERDSLUITEN = 2,
    RESETESTOP = 3
};

enum terugkoppeling {
    WORDTGEOPEND = 0,
    WORDTGESLOTEN = 1,
    GEOPEND = 2,
    GESLOTEN = 3,
    GEFORCEERDGESLOTEN = 4,
    STILGEZETTIJDENSBEWEGEN = 5,
    WORDTGEFORCEERDGESLOTEN = 6
};

enum veranderProtocolVKL {
    UIT = 0,
    ALLEENROOD = 1,
    ALLEENORANJE = 2,
    ALLEENGROEN = 3,
    ALLEENORANJEKNIPPEREN = 4,
};


#define MSG_BUFFER_SIZE (50)
char msg[MSG_BUFFER_SIZE];
WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;

bool eStopActive = false;
uint8_t asbServoPin = 26;
uint8_t asbLedPin1 = 33;
uint8_t asbLedPin2 = 32;
// ServoBarrier servo(asbServoPin, asbLedPin);

void processEstopActivate() {
    client.publish(ASB_MESSAGE_TOPIC, "5");
    Serial.println("E-stop is geactiveerd.");
}

auto ultrasoon = new Ultrasonic(13, 12);
barrierData asbConfig = {
    asbServoPin,
    asbLedPin1,
    asbLedPin2,
    LANE_WIDTH_CM,
    ultrasoon,
    processEstopActivate
};
auto servo = new ServoBarrier(asbConfig);
auto stopLight = new StopLight(21, 19, 18);

const char* ntpServer = "pool.ntp.org";
const long  gmtOffset_sec = 3600;
const int   daylightOffset_sec = 0;
bool timeIsSynced = false;

void syncTimeWithServer() {
    configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
    struct tm timeinfo;

    do {
        configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
    } while (!getLocalTime(&timeinfo));

    Serial.println("Time is synced with server");
    timeIsSynced = true;
}

// Function prints the current time
// void printLocalTime() {
//   struct tm timeinfo;
//   if (!getLocalTime(&timeinfo)) {
//     Serial.println("Failed to obtain time");
//     return;
//   }
//   Serial.println(&timeinfo, "%A, %B %d %Y %H:%M:%S");
// }

bool readyForNextSensorReading() {
    static struct timeval timeStruct;
    static uint16_t lastRoundedMs = 0;
    gettimeofday(&timeStruct, NULL);

    uint16_t currentMs = timeStruct.tv_usec / 1000;

    if (currentMs >= lastRoundedMs + 200 - 15 && currentMs <= lastRoundedMs + 200 + 15) {
        lastRoundedMs = (currentMs + 20) / 100 * 100;
        return true;
    }

    return false;
}
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


void moveBarrier(bool pos) {
    pos ? servo->setRequestedPositionUp() : servo->setRequestedPositionDown();
}


void processIncomingASBMessage(uint8_t message) {
    switch (message) {
        case veranderProtocolASB::OPENEN:
            servo->setRequestedPositionUp();
            client.publish(ASB_MESSAGE_TOPIC, "0");
            Serial.println("Slagboom wordt geopend.");
            break;
        case veranderProtocolASB::NORMAALSLUITEN:
            servo->setRequestedPositionDown();
            client.publish(ASB_MESSAGE_TOPIC, "1");
            Serial.println("Slagboom wordt gesloten.");
            break;
        case veranderProtocolASB::GEFORCEERDSLUITEN:
            servo->setRequestedPositionDown();
            servo->moveBarrierForced();
            client.publish(ASB_MESSAGE_TOPIC, "1");
            Serial.println("Slagboom wordt geforceerd gesloten.");
            break;
        case veranderProtocolASB::RESETESTOP:
            servo->setRequestedPositionDown();
            servo->setEstopStatus(false);
            client.publish(ASB_MESSAGE_TOPIC, "1");
            Serial.println("Slagboom wordt geforceerd gesloten.");
            break;
        default:
            client.publish(ASB_MESSAGE_TOPIC, "6");
            Serial.println("Bericht is geen commando.");
            break;
    }
}


void processIncomingVKLMessage(uint8_t message) {
    switch (message) {
        case veranderProtocolVKL::UIT:
            stopLight->setAllLightsOff();
            Serial.println("Turned off all lights.");
            break;
        case veranderProtocolVKL::ALLEENROOD:
            stopLight->setAllLightsOff();
            stopLight->setLight(Colors::red, true);
            Serial.println("Turned all lights off and set red light on.");
            break;
        case veranderProtocolVKL::ALLEENORANJE:
            stopLight->setAllLightsOff();
            stopLight->setLight(Colors::orange, true);
            Serial.println("STurned all lights off and set orange light on.");
            break;
        case veranderProtocolVKL::ALLEENGROEN:
            stopLight->setAllLightsOff();
            stopLight->setLight(Colors::green, true);
            Serial.println("Turned all lights off and set green light on.");
            break;
        case veranderProtocolVKL::ALLEENORANJEKNIPPEREN:
            stopLight->setAllLightsOff();
            stopLight->setOrangeBlinking(true);
            Serial.println("Turned all lights off and set orange light to blinking.");
            break;
        default:
            client.publish(VKL_MESSAGE_TOPIC, "6");
            Serial.println("Bericht is geen commando.");
    }
}



void callback(char* topic, byte* payload, unsigned int length) {
    char message[length + 1]; // +1 for null terminator
    for (int i = 0; i < length; i++) {
        message[i] = (char)payload[i];
    }
    message[length] = '\0';
    // for debugging
    // Serial.printf("Message arrived [%s] %s\n", topic, message);

    if (strcmp(topic, ASB_INCOMING_TOPIC) == 0) {
        processIncomingASBMessage(message[0] - '0');
    }


    if (strcmp(topic, VKL_INCOMING_TOPIC) == 0) {
        processIncomingVKLMessage(message[0] - '0');
    }

    // Print out the message for debugging
    if (strcmp(message, "test") == 0) {
        Serial.println("Test message received");
        client.publish(ASB_MESSAGE_TOPIC, "Test message received");
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
            client.publish(BOT_MESSAGE_TOPIC, "Bot connected!");

            // ... and resubscribe
            client.subscribe(ASB_INCOMING_TOPIC);
            client.subscribe(VKL_INCOMING_TOPIC);
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
    pinMode(BUILTIN_LED, OUTPUT);
    Serial.begin(115200);

    setup_wifi();
    client.setServer(mqtt_server, MQTT_PORT);
    client.setCallback(callback);
    servo->setRequestedPositionUp();
    servo->moveBarrierForced();
    syncTimeWithServer();
}

void loop() {
    if (!client.connected()) {
        delay(2000);
        reconnect();
    }
    client.loop();

    static uint8_t updateTimeCounter = 0;
    static Timer *updateTimeTimer = new Timer(SET_TIMER_IN_MS);

    if (updateTimeTimer->waitTime(60000)) {
        updateTimeCounter++;
        if (updateTimeCounter >= 5) {
            syncTimeWithServer();
            updateTimeCounter = 0;
        }
    }

    if (readyForNextSensorReading()) {
        servo->callback();
    }
    stopLight->callback();
}
