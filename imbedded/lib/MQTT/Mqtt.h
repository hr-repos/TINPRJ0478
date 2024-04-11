#pragma once
#ifndef _MQTT_H_
#define _MQTT_H_

#include "Arduino.h"
#include <WiFi.h>
#include <PubSubClient.h>
#include <time.h>

#define PUBLISH_TOPIC "data/asb1"
#define SUBSCRIBE_TOPIC "asb/1"
#define SUBSCRIBE_TOPIC_VKL "vkl/#"

class Mqtt
{
private:
    const char *BOT_ID;
    const char *ssid;
    const char *password;

    const char *MqttUser;
    const char *MqttPass;

    WiFiClient espClient;
    PubSubClient *client;

    void (*pubSub)(PubSubClient *client);

public:
    Mqtt(char *ssid, char *password, char *ID, char *MqttUser, char *MqttPass);
    ~Mqtt();

    void connectMqtt(char *mqttServer, int mqttPort, void (*pubSub)(PubSubClient *client), void (*myCallBack)(PubSubClient *client, char *topic, char *message, unsigned int length));
    void printLocalTime();
    void reconnect();

    PubSubClient *getClient();

private:
    void setupWifi();
};

#endif