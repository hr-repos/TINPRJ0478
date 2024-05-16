#include "StopLight.h"
#include "Timer/Timer.h"

StopLight::StopLight(uint8_t pinRedLed, uint8_t pinOrangeLed, uint8_t pinGreenLed)
    : pinRedLed(pinRedLed), pinOrangeLed(pinOrangeLed), pinGreenLed(pinGreenLed) {
    pinMode(pinRedLed, OUTPUT);
    pinMode(pinOrangeLed, OUTPUT);
    pinMode(pinGreenLed, OUTPUT);
    orangeBlinking = false;
}

void StopLight::setLicht(Colors color, bool ledOn) {
    uint8_t pin = 0;

    switch (color) {
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

void StopLight::setOrangeBlinking(bool blinking) {
    orangeBlinking = blinking;
}

void StopLight::callback() {
    static Timer blinktimer = new Timer(SET_TIMER_IN_MS);

    if (orangeBlinking) {
        if (blinktimer.waitTime(500)) {
            digitalWrite(pinOrangeLed, !digitalRead(pinOrangeLed));
        }
    }
}
