#include <Arduino.h>
#include "ServoBarrier.h"

uint8_t servoPin = 26;
auto servo = new ServoBarrier(servoPin);

void setup()
{
    Serial.begin(115200);
}

void loop()
{
    servo->setLocationDown();     
    Serial.println("set servo down");
    delay(3000);
    servo->setLocationUp();    
    Serial.println("set servo up");
    delay(3000);
}