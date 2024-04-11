#include "Ultrasonic.h"

static int readDistance = 0;

Ultrasonic::Ultrasonic(uint8_t triggerPin, uint8_t echoPin)
{
  this->triggerPin = triggerPin;
  this->echoPin = echoPin;

  ultrasoonStartup();
}

void Ultrasonic::ultrasoonStartup()
{
  pinMode(triggerPin, OUTPUT);
  pinMode(echoPin, INPUT);
}

int Ultrasonic::readUltrasoon_cm()
{
  static Timer *timer_us = new Timer(SET_TIMER_IN_US);

  static int step = 0;
  static int mode = begin;

  if (timer_us->waitTime(1))
    step++;

  switch (mode)
  {
  case begin:
    digitalWrite(triggerPin, LOW);
    mode = sendSound;
    break;

  case sendSound:
    if (step >= 2)
    {
      digitalWrite(triggerPin, HIGH);
      mode = readTheDistance;
    }
    break;

  case readTheDistance:
    if (step >= 10)
    {
      digitalWrite(triggerPin, LOW);

      int sensorOutput = pulseIn(echoPin, HIGH);
      int distance = CALULATE_DISTANCE(sensorOutput);

      mode = reset;
      return distance;
    }
    break;

  case reset:
    step = 0;
    mode = begin;
    break;

  default:
    break;
  }

  return READING_NOT_FOUND;
}

bool Ultrasonic::ultrasoonDetectAtDistance_cm(int distance_cm)
{
  static int timer = 0;
  static int safetyBuffer = 0;
  int newDistance = readUltrasoon_cm();

  if (newDistance == READING_NOT_FOUND)
    return false;

  if (newDistance != 0)
    readDistance = newDistance;

  timer++;

  if (newDistance <= distance_cm && readDistance != 0)
  {
    safetyBuffer++;
  }
  else if (timer >= 10)
  {
    timer = 0;
    safetyBuffer = 0;
  }

  if (safetyBuffer >= 4)
  {
    safetyBuffer = 0;
    return true;
  }

  return false;
}