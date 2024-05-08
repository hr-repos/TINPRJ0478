#include "ServoBarrier.h"
#include "Timer/Timer.h"

ServoBarrier::ServoBarrier(barrierData config)
    : config(config)
{
    pinMode(config.servoPin, OUTPUT);
    pinMode(config.ledPin1, OUTPUT);
    pinMode(config.ledPin2, OUTPUT);

    ESP32PWM::allocateTimer(0);
    ESP32PWM::allocateTimer(1);
    ESP32PWM::allocateTimer(2);
    ESP32PWM::allocateTimer(3);
    servo.setPeriodHertz(3);
    servo.attach(config.servoPin);
}

bool ServoBarrier::objectDetected()
{
    return config.sonic->readUltrasonic_cm() < config.laneWidth;
}

void ServoBarrier::setServoPos(uint64_t pos)
{
    servoPos = pos;
    servo.write(servoPos);
}

void ServoBarrier::setLocationDown()
{
    servoPos = closingPosition;
    servo.write(servoPos);
}

void ServoBarrier::setLocationUp()
{
    servoPos = openingPosition;
    servo.write(servoPos);

    digitalWrite(config.ledPin1, LOW);
    digitalWrite(config.ledPin2, LOW);
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
    digitalWrite(config.ledPin1, ledSwitch);
    digitalWrite(config.ledPin2, !ledSwitch);
    ledSwitch = !ledSwitch;
}

void ServoBarrier::moveBarrier() {
    if (requestedPosition < currentPosition) {
        currentPosition -= servoStepSize;
    }
    else {
        currentPosition += servoStepSize;
    }
    servo.write(currentPosition);
}

void ServoBarrier::callback()
{
    static Timer *ledTimer = new Timer(SET_TIMER_IN_MS);
    static Timer *servoTimer = new Timer(SET_TIMER_IN_MS);
    if (!isDown())
        return;

    if (ledTimer->waitTime(500))
    {
        switchLeds();
    }


    if (requestedPosition != currentPosition) {
        
        if (ledTimer->waitTime(25))
        {
            moveBarrier();
        }
    }
}
