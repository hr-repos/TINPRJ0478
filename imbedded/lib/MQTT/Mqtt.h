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
    const String BOT_ID;
    const String ssid;
    const String password;

    const String MqttUser;
    const String MqttPass;

    WiFiClient espClient;
    PubSubClient *client;

    void (*pubSub)(PubSubClient *client);

public:
    Mqtt(String ssid, String password, String ID, String MqttUser, String MqttPass);
    ~Mqtt();

    void connectMqtt(String mqttServer, int mqttPort, void (*pubSub)(PubSubClient *client), void (*myCallBack)(PubSubClient *client, char *topic, char *message, unsigned int length));
    void printLocalTime();
    void reconnect();

    PubSubClient *getClient();

private:
    void setupWifi();
};

#endif