#pragma once
#ifndef SERVOBARRIER_H
#define SERVOBARRIER_H

#include <Arduino.h>
#include <ESP32Servo.h>

class ServoBarrier
{
private:
    u_int8_t servoPos; // current position of the servo
    u_int8_t servoPin; // pin of the servo
    Servo servo;       // servo object

public:
    ServoBarrier(u_int8_t servoPin); // create object and set the pin for the stepper motor

    void setServoPos(uint64_t pos); // set the position of the servo
    void setLocationUp();           // set the position of the servo to 25
    void setLocationDown();         // set the position of the servo to 100
    u_int8_t getLocation();         // get current location of the servo
};

#endif