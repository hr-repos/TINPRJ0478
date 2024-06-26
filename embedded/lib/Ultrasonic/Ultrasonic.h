#ifndef __ULTRASONIC_H__
#define __ULTRASONIC_H__

#include <Arduino.h>
#include "Timer/Timer.h"

#define READING_NOT_FOUND -69
#define CALCULATE_DISTANCE(outputSensor) (int)((double)outputSensor * 0.01715)

enum ultrasonicMode
{
    begin           = 0,
    sendSound       = 1,
    readTheDistance = 2,
    reset           = 3
};

class Ultrasonic
{
private:
    uint8_t echoPin;
    uint8_t triggerPin;

public:
    Ultrasonic(uint8_t triggerPin, uint8_t echoPin);

    bool ultrasonicDetectAtDistance_cm(int distance_cm);
    int readUltrasonic_cm();

private:
    void ultrasonicStartup();
};

#endif