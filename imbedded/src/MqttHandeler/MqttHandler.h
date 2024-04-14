#pragma once
#ifndef _MQTTHANDLER_H
#define _MQTTHANDLER_H

#include "Mqtt.h"
#include <WiFi.h>
#include <time.h>
#include "secret.h"
#include <Arduino.h>
#include "StopLight.h"
#include "Ultrasonic.h"
#include <PubSubClient.h>
#include "ServoBarrier.h"

void MqttSetup();
void MqttLoop();

#endif