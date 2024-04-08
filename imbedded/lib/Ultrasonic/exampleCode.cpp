#include <Arduino.h>
#include "SonicPair/SonicPair.h"

void runExample() 
{
  Serial.begin(9600);
  Serial.println("started");

  const uint8_t triggerPin1 = 9;
  const uint8_t echoPin1 = 8;

  const uint8_t triggerPin2 = 11;
  const uint8_t echoPin2 = 10;

  auto Utrasonic1 = new Ultrasonic(triggerPin1, echoPin1);
  auto Utrasonic2 = new Ultrasonic(triggerPin2, echoPin2);

  auto sonicPair  = new SonicPair(Utrasonic1, Utrasonic2);

  while(true)
  {
    int* dist = sonicPair->readDistances();

    //if reading is not done then dist = nullptr
    if(dist != nullptr)
    {      
      String line = "ultra1: " + String(dist[0]) + ", " + "ultra2: " + String(dist[1]);
      Serial.println(line);
    }
  }

  delete Utrasonic1;
  delete Utrasonic2;
  delete sonicPair;
}