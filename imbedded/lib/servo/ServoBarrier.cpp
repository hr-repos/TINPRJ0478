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
    requestedPosition = pos;
}

void ServoBarrier::setRequestedPositionDown()
{
    requestedPosition = closingPosition;
}

void ServoBarrier::setRequestedPositionUp()
{
    requestedPosition = openingPosition;
}

u_int8_t ServoBarrier::getLocation()
{
    return currentPosition;
}

bool ServoBarrier::isDown()
{
    return currentPosition == closingPosition;
}

void ServoBarrier::switchLeds()
{
    digitalWrite(config.ledPin1, ledSwitch);
    digitalWrite(config.ledPin2, !ledSwitch);
    ledSwitch = !ledSwitch;
}

void ServoBarrier::moveBarrier() {
    if (objectDetected()) {
        eStopActive = true;
        config.EStopHandlerMainLoop();
        return;
    }

    if (requestedPosition < currentPosition) {
        currentPosition -= servoStepSize;
    }
    else {
        currentPosition += servoStepSize;
    }
    servo.write(currentPosition);
}

bool ServoBarrier::getEstopStatus() {
    return eStopActive;
}

void ServoBarrier::setEstopStatus(bool newStatus) {
    eStopActive = newStatus;
}

void ServoBarrier::moveBarrierForced() {
    currentPosition = requestedPosition;
    servo.write(currentPosition);
}

void ServoBarrier::callback()
{
    static Timer *ledTimer = new Timer(SET_TIMER_IN_MS);
    static Timer *servoTimer = new Timer(SET_TIMER_IN_MS);

    if (requestedPosition != currentPosition && !eStopActive) {

        if (servoTimer->waitTime(15))
        {
            moveBarrier();
        }
    }

    if (!isDown()) {
        digitalWrite(config.ledPin1, LOW);
        digitalWrite(config.ledPin2, LOW);
        return;
    }

    if (ledTimer->waitTime(500))
    {
        switchLeds();
    }


}
