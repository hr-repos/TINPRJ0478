#include "SonicPair.h"

SonicPair::SonicPair(Ultrasonic *ultrasonicOne, Ultrasonic *ultrasonicTwo)
    : ultrasonicArray(new Ultrasonic *[2]{ ultrasonicOne, ultrasonicTwo })
{
}

SonicPair::~SonicPair()
{
    if (ultrasonicArray != nullptr)
        delete[] ultrasonicArray;

    if (distanceArray != nullptr)
        delete[] distanceArray;
}

int *SonicPair::readDistances()
{
    static uint8_t switchCounter = 0;
    static bool switchSonic = false;

    int distance;

    distance = ultrasonicArray[switchSonic]->readUltrasonic_cm();

    if (distance != READING_NOT_FOUND)
    {
        distanceArray[switchSonic] = distance;

        switchSonic = !switchSonic;
        switchCounter++;
    }

    if (switchCounter >= 3)
    {
        switchCounter = 0;
        return distanceArray;
    }

    return nullptr;
}