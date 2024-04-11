#pragma once
#ifndef SERVOBARRIER_H
#define SERVOBARRIER_H

#include <Arduino.h>
#include <ESP32Servo.h>
#include "Timer/Timer.h"
#include "./Ultrasonic.h"
#include <StopLight.h>

class ServoBarrier
{
private:
    uint8_t ledPin1;  
    uint8_t ledPin2;      
    uint8_t servoPin;       
    uint8_t servoPos;       // current position of the servo
    Servo servo;           
    bool ledSwitch = false;
    Ultrasonic* sonic;

    uint8_t closingPosition = 106;
    uint8_t openingPosition = 25;

    void switchLeds();      // switch the leds of the barrier

public:
    // create object and set the pin for the stepper motor
    ServoBarrier(uint8_t servoPin, uint8_t ledPin1, uint8_t ledPin2, Ultrasonic* sonic);

    // update the leds of the barrier
    void callback();   

    // set the position of the servo             
    void setServoPos(uint64_t pos); 

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