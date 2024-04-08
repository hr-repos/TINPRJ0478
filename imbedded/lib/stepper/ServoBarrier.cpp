#include "ServoBarrier.h"

// create object and set the pin for the stepper motor
ServoBarrier::ServoBarrier(uint8_t pin) : servoPin(pin)
{
    pinMode(pin, OUTPUT);
    ESP32PWM::allocateTimer(0);
    ESP32PWM::allocateTimer(1);
    ESP32PWM::allocateTimer(2);
    ESP32PWM::allocateTimer(3);
    servo.setPeriodHertz(50); 
    servo.attach(servoPin);   
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
    servo.write(100);
}

// set new servo location to up (25)
void ServoBarrier::setLocationUp()
{
    servo.write(25);
}

// get current location of the servo
u_int8_t ServoBarrier::getLocation() {
    return servoPos;
}