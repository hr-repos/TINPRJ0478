#include <Arduino.h>
#include "ServoBarrier.h"

uint8_t servoPin = 26;
auto servo = new ServoBarrier(servoPin, LED_BUILTIN);
static Timer *timer = new Timer(SET_TIMER_IN_MS);

void setup()
{
    Serial.begin(115200);
    servo->setLocationDown();
    Serial.println("set servo down");
}

void loop()
{

    if (timer->waitTime(5000))
    {
        if (servo->isDown())
        {
            servo->setLocationUp();
        }
        else
        {
            servo->setLocationDown();
        }
    }

    servo->callback();
}