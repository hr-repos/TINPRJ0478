#include "StopLight.h"

StopLight::StopLight(uint8_t pinRedLed, uint8_t pinOrangeLed, uint8_t pinGreenLed)
: pinRedLed(pinRedLed), pinOrangeLed(pinOrangeLed), pinGreenLed(pinGreenLed)
{
    pinMode(pinRedLed, OUTPUT);
    pinMode(pinOrangeLed, OUTPUT);
    pinMode(pinGreenLed, OUTPUT);
}

void StopLight::setLicht(Colors color, bool ledOn)
{
    uint8_t pin = 0;

    switch(color)
    {
        case red:
            pin = pinRedLed;
            break;

        case orange:
            pin = pinOrangeLed;
            break;

        case green:
            pin = pinGreenLed;
            break;

        default:
            return;
    }

    digitalWrite(pinRedLed, LOW);
    digitalWrite(pinOrangeLed, LOW);
    digitalWrite(pinGreenLed, LOW);

    digitalWrite(pin, ledOn);
}