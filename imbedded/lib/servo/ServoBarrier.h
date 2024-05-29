#pragma once
#ifndef SERVOBARRIER_H
#define SERVOBARRIER_H

#include <Arduino.h>
#include <ESP32Servo.h>
#include <PubSubClient.h>
#include <StopLight.h>
#include "Ultrasonic.h"

struct barrierData {
    uint8_t servoPin;
    uint8_t ledPin1;
    uint8_t ledPin2;
    uint8_t laneWidth;
    Ultrasonic *sonic;
    void (*EStopHandlerMainLoop)();
};

class ServoBarrier
{
private:
    uint8_t servoPos;  // current position of the servo
    Servo servo;
    bool ledSwitch = false;
    bool eStopActive = false;

    barrierData config;


    // make sure the clsoingposition and opening position
    // values can be devided by the stepsize
    const uint8_t servoStepSize = 5;
    uint8_t currentPosition = 115;
    uint8_t requestedPosition = 115;

    void switchLeds();      // switch the leds of the barrier
    void moveBarrier();     // moves the barrier if it's not at the requested location

public:
    // create object and set the pin for the stepper motor
    explicit ServoBarrier(barrierData);

    const uint8_t closingPosition = 25;
    const uint8_t openingPosition = 115;

    // returns if the sonic detects an
    bool objectDetected();

    // update the leds of the barrier
    void callback();

    // set the position of the servo
    void setServoPos(uint64_t);

    // set the position of the servo to 25
    void setRequestedPositionUp();

    // set the position of the servo to 100
    void setRequestedPositionDown();

    // move the barrier immediately without checking for objects
    void moveBarrierForced();

    // get boolean if the servo is down
    bool isUp();


    bool getEstopStatus();
    void setEstopStatus(bool newStatus);

    // get current location of the servo
    uint8_t getLocation();
};

#endif
