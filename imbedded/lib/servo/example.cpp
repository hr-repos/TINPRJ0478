#include <Arduino.h>
#include "ServoBarrier.h"

uint8_t servoPin = 26;
uint8_t dummyLed1 = 69;
uint8_t dummyLed2 = 69;
uint8_t laneWidth= 69;
auto sonic = new Ultrasonic(1, 2);
barrierData asbConfig = {servoPin, dummyLed1, dummyLed2, laneWidth, sonic};
auto servo = new ServoBarrier(asbConfig);
static Timer *timer = new Timer(SET_TIMER_IN_MS);

void setup()
{
    Serial.begin(115200);
    servo->setRequestedPositionDown();
    Serial.println("set servo down");
}

void loop()
{

    if (timer->waitTime(5000))
    {
        if (servo->isUp())
        {
            // servo->setLocationUp();
        }
        else
        {
            servo->setRequestedPositionDown();
        }
    }

    servo->asbMoveCallback();
}

