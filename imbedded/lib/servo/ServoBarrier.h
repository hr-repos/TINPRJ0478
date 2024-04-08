#pragma once
#ifndef SERVOBARRIER_H
#define SERVOBARRIER_H

#include <Arduino.h>
#include <ESP32Servo.h>
#include "Timer/Timer.h"

class ServoBarrier
{
private:
    u_int8_t ledPin;   // pin of the servo
    u_int8_t servoPin; // pin of the servo
    u_int8_t servoPos; // current position of the servo
    Servo servo;       // servo object
    bool ledsOn = false;

    void switchLeds();              // switch the leds of the barrier

public:
    ServoBarrier(u_int8_t servoPin, u_int8_t ledPin); // create object and set the pin for the stepper motor

    void callback();                // update the leds of the barrier
    void setServoPos(uint64_t pos); // set the position of the servo
    void setLocationUp();           // set the position of the servo to 25
    void setLocationDown();         // set the position of the servo to 100
    bool isDown();                  // get boolean if the servo is down
    u_int8_t getLocation();         // get current location of the servo
    
};

#endif