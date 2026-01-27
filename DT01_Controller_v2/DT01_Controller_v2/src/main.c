/*
 * Senior Design Team 1
 * University of Akron
 * Department of Electrical and Computer Engineering
 * Members: Jacob Dye, Alex Kinch, and Josh Thum
 * Project: Ultrasonic Parking Sensor (Parking Information Supervision System)
 * Objective: To sense vehicles and light a status light to match.
 */

// The following lines are from DT01_Controller/main.c
#include <Arduino.h>
#include "Ethernet_Generic.h"

//define macros for ultrasonic senor
#define trigger RA3
#define echo    RB5

//define macros for red and green LEDs
#define rLED    RB1
#define gLED    RB2

//serial stuff
#define START_DATA_STREAM_PROTOCOL 0x03
#define STOP_DATA_STREAM_PROTOCOL 0xFC

//define distance calculation function
int calc_dist(void);

void main(void)
{
    // Initialize the device
    SYSTEM_Initialize();
    
    // -- [[ Configure Timer1 To Operate In Timer Mode  ]] --
 
    // Clear The Timer2 Register. To start counting from 0
    TMR2 = 0;
    // Choose the local clock source (timer mode)
    //TMR2CS = 0;
    // Choose the desired prescaler ratio (1:1)
    T2CKPS0 = 0;
    T2CKPS1 = 0;
    
    rLED=1;
    gLED=1;
    __delay_ms(500);
    rLED=0;
    gLED=0;

    // define variables needed in while loop so they are global
    uint8_t occupied=0;
    uint8_t error=0;
    
    while (1)
    {
        // loop for checking status and setting lights accordingly
        int dist = calc_dist();
        
        if (dist <= 512){
            rLED=1;
            gLED=0;
            occupied=1;
            if (error == 1){
                gLED=1;
            }
        } else {
            rLED=0;
            gLED=1;
            occupied=0;
            if (error == 1){
                rLED=1;
            }
        }
        
        __delay_ms(250);
    }
}

// Definition Of The calc_dist() Function
int calc_dist(void)
{
  // define required variables
  unsigned long int distance=0;
  unsigned long int timer=0;
  // set timer to zero
  TMR2=0;
  // Send Trigger Pulse To The Sensor
  trigger=1;
  // pulse is 10 us long
  __delay_us(10);
  // turn off pulse
  trigger=0;
  // Wait For The Echo Pulse From The Sensor
  while(!echo);
  // Turn ON Timer Module
  TMR2ON=1;
  // Wait Until The Pulse Ends
  while(echo);
  // Turn OFF The Timer
  TMR2ON=0;
  // Calculate The Distance Using The Equation
  distance=TMR2/58.82;
  timer=TMR2;
  
  return distance;
}