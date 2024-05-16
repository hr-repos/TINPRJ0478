#pragma once
#ifndef _STOPLIGHT_H_
#define _STOPLIGHT_H_

#include "Arduino.h"

enum Colors {
    red,
    orange,
    green
};

class StopLight {
 private:
    uint8_t pinRedLed;
    uint8_t pinOrangeLed;
    uint8_t pinGreenLed;
    bool orangeBlinking;

 public:
    StopLight(uint8_t pinRedLed, uint8_t pinOrangeLed, uint8_t pinGreenLed);

    void setLicht(Colors color, bool ledOn);
    void setOrangeBlinking(bool blinking);
    void callback();
};

#endif
