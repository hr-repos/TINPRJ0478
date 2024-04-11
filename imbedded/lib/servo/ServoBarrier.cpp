#include "ServoBarrier.h"
#include "Timer/Timer.h"


ServoBarrier::ServoBarrier(uint8_t servoPin, uint8_t ledPin1, uint8_t ledPin2, Ultrasonic* sonic) 
: servoPin(servoPin), ledPin1(ledPin1), ledPin2(ledPin2), sonic(sonic)
{
    pinMode(servoPin, OUTPUT);
    pinMode(ledPin1, OUTPUT);
    pinMode(ledPin2, OUTPUT);

    ESP32PWM::allocateTimer(0);
    ESP32PWM::allocateTimer(1);
    ESP32PWM::allocateTimer(2);
    ESP32PWM::allocateTimer(3);
    servo.setPeriodHertz(3);
    servo.attach(servoPin);
}

void ServoBarrier::setServoPos(uint64_t pos)
{
    servoPos = pos;
    servo.write(servoPos);
}


void ServoBarrier::setLocationDown()
{
    //if(sonic->ultrasoonDetectAtDistance_cm(20));
       //return;

    servoPos = closingPosition;
    servo.write(servoPos);
}


void ServoBarrier::setLocationUp()
{
    servoPos = openingPosition;
    servo.write(servoPos);


    digitalWrite(ledPin1, LOW);
    digitalWrite(ledPin2, LOW);
}

u_int8_t ServoBarrier::getLocation()
{
    return servoPos;
}


bool ServoBarrier::isDown()
{
    return servoPos == closingPosition;
}


void ServoBarrier::switchLeds()
{
    digitalWrite(ledPin1, ledSwitch);
    digitalWrite(ledPin2, !ledSwitch);
    ledSwitch = !ledSwitch;
}


void ServoBarrier::callback()
{
    static Timer *timer = new Timer(SET_TIMER_IN_MS);
    if (!isDown())
        return;

    if (timer->waitTime(500))
    {
        switchLeds();
    }
}