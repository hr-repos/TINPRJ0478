#include <Arduino.h>


#define MSG_BUFFER_SIZE (50)

unsigned long lastMsg = 0;
char msg[MSG_BUFFER_SIZE];


// const char* ntpServer = "pool.ntp.org";
// const long  gmtOffset_sec = 3600;
// const int   daylightOffset_sec = 0;


void setup()
{
    pinMode(BUILTIN_LED, OUTPUT); // Initialize the BUILTIN_LED pin as an output
    Serial.begin(115200);
}

void loop()
{
}
