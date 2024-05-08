#pragma once
#ifndef SERVOBARRIER_H
#define SERVOBARRIER_H

#include <Arduino.h>
#include <ESP32Servo.h>
#include "Ultrasonic.h"
#include <StopLight.h>

struct barrierData {
    uint8_t servoPin;
    uint8_t ledPin1;
    uint8_t ledPin2;
    uint8_t laneWidth;
    Ultrasonic *sonic;
};

class ServoBarrier
{
private:
    uint8_t servoPos;  // current position of the servo
    Servo servo;
    bool ledSwitch = false;

    barrierData config;


    // make sure the clsoingposition and opening position
    // values can be devided by the stepsize
    const uint8_t servoStepSize = 5;
    uint8_t currentPosition = 25;
    uint8_t requestedPosition = 25;

    void switchLeds();      // switch the leds of the barrier
    void moveBarrier();     // moves the barrier if it's not at the requested location

public:
    // create object and set the pin for the stepper motor
    ServoBarrier(uint8_t servoPin, uint8_t ledPin1, uint8_t ledPin2, Ultrasonic *sonic, uint8_t laneWidth);
    explicit ServoBarrier(barrierData);

    const uint8_t closingPosition = 105;
    const uint8_t openingPosition = 25;

    // returns if the sonic detects an
    bool objectDetected();

    // update the leds of the barrier
    void callback();

    // set the position of the servo
    void setServoPos(uint64_t);

    // set the position of the servo to 25
    void setLocationUp();

    // set the position of the servo to 100
    void setLocationDown();

    // get boolean if the servo is down
    bool isDown();

    // get current location of the servo
    uint8_t getLocation();
};

#endif
