#include "ServoBarrier.h"
#include "Timer/Timer.h"

// create object and set the pin for the stepper motor
ServoBarrier::ServoBarrier(uint8_t servoPin, u_int8_t ledPin) : servoPin(servoPin), ledPin(ledPin)
{
    pinMode(servoPin, OUTPUT);
    pinMode(ledPin, OUTPUT);
    ESP32PWM::allocateTimer(0);
    ESP32PWM::allocateTimer(1);
    ESP32PWM::allocateTimer(2);
    ESP32PWM::allocateTimer(3);
    servo.setPeriodHertz(50);
    servo.attach(servoPin);
    //timer_us = new Timer(SET_TIMER_IN_MS);
}

// set new servo location
void ServoBarrier::setServoPos(uint64_t pos)
{
    servoPos = pos;
    servo.write(servoPos);
}

// set new servo location to down (100)
void ServoBarrier::setLocationDown()
{
    servoPos = 100;
    servo.write(servoPos);
}

// set new servo location to up (25)
void ServoBarrier::setLocationUp()
{
    servoPos = 25;
    servo.write(servoPos);
    digitalWrite(ledPin, LOW);
}

// get current location of the servo
u_int8_t ServoBarrier::getLocation()
{
    return servoPos;
}

// get current location of the servo
bool ServoBarrier::isDown()
{
    return servoPos == 100;
}

// switch the leds of the barrier on or off
void ServoBarrier::switchLeds()
{
    if (ledsOn)
    {
        digitalWrite(ledPin, LOW);
        ledsOn = false;
    }
    else
    {
        digitalWrite(ledPin, HIGH);
        ledsOn = true;
    }
}

// get current location of the servo
void ServoBarrier::callback()
{
    static Timer* timer = new Timer(SET_TIMER_IN_MS);
    if (isDown())
    {
        if (timer->waitTime(500))
        {
            switchLeds();
        }
    }
}