#ifndef __SONICPAIR_H__
#define __SONICPAIR_H__

#include "Ultrasonic/Ultrasonic.h"

class SonicPair
{
private:
    Ultrasonic** ultrasonicArray;
    int* distanceArray = new int[2];

public:
    SonicPair(Ultrasonic* ultrasonicOne, Ultrasonic* ultrasonicTwo);
    ~SonicPair();

    //will send nullptr if reading is not done and int[2](where [0]=ultrasonic1.distance, [1]=ultrasonic2.distance) if reading is done
    int* readDistances();
};

#endif