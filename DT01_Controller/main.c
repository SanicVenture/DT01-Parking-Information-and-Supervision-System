/**
  Generated Main Source File

  Company:
    Microchip Technology Inc.

  File Name:
    main.c

  Summary:
    This is the main file generated using PIC10 / PIC12 / PIC16 / PIC18 MCUs

  Description:
    This header file provides implementations for driver APIs for all modules selected in the GUI.
    Generation Information :
        Product Revision  :  PIC10 / PIC12 / PIC16 / PIC18 MCUs - 1.81.8
        Device            :  PIC18F67J60
        Driver Version    :  2.00
*/

/*
    (c) 2018 Microchip Technology Inc. and its subsidiaries. 
    
    Subject to your compliance with these terms, you may use Microchip software and any 
    derivatives exclusively with Microchip products. It is your responsibility to comply with third party 
    license terms applicable to your use of third party software (including open source software) that 
    may accompany Microchip software.
    
    THIS SOFTWARE IS SUPPLIED BY MICROCHIP "AS IS". NO WARRANTIES, WHETHER 
    EXPRESS, IMPLIED OR STATUTORY, APPLY TO THIS SOFTWARE, INCLUDING ANY 
    IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY, AND FITNESS 
    FOR A PARTICULAR PURPOSE.
    
    IN NO EVENT WILL MICROCHIP BE LIABLE FOR ANY INDIRECT, SPECIAL, PUNITIVE, 
    INCIDENTAL OR CONSEQUENTIAL LOSS, DAMAGE, COST OR EXPENSE OF ANY KIND 
    WHATSOEVER RELATED TO THE SOFTWARE, HOWEVER CAUSED, EVEN IF MICROCHIP 
    HAS BEEN ADVISED OF THE POSSIBILITY OR THE DAMAGES ARE FORESEEABLE. TO 
    THE FULLEST EXTENT ALLOWED BY LAW, MICROCHIP'S TOTAL LIABILITY ON ALL 
    CLAIMS IN ANY WAY RELATED TO THIS SOFTWARE WILL NOT EXCEED THE AMOUNT 
    OF FEES, IF ANY, THAT YOU HAVE PAID DIRECTLY TO MICROCHIP FOR THIS 
    SOFTWARE.
*/

#include "mcc_generated_files/mcc.h"

/*
                         Main application
 */

//define macros for ultrasonic senor
#define trigger RA3
#define echo    RB5

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
 
    // Clear The Timer1 Register. To start counting from 0
    TMR1 = 0;
    // Choose the local clock source (timer mode)
    TMR1CS = 0;
    // Choose the desired prescaler ratio (1:1)
    T1CKPS0 = 0;
    T1CKPS1 = 0;
    
    // If using interrupts in PIC18 High/Low Priority Mode you need to enable the Global High and Low Interrupts
    // If using interrupts in PIC Mid-Range Compatibility Mode you need to enable the Global and Peripheral Interrupts
    // Use the following macros to:

    // Enable the Global Interrupts
    //INTERRUPT_GlobalInterruptEnable();

    // Disable the Global Interrupts
    //INTERRUPT_GlobalInterruptDisable();

    // Enable the Peripheral Interrupts
    //INTERRUPT_PeripheralInterruptEnable();

    // Disable the Peripheral Interrupts
    //INTERRUPT_PeripheralInterruptDisable();
    //int distance = calc_distance();
    rLED=1;
    gLED=1;
    //_delay((unsigned long)(0.5*32000/4));
    __delay_ms(500);
    rLED=0;
    gLED=0;
    //printf("%s","I am Testing microcontroller");
    uint8_t occupied=0;
    
    while (1)
    {
        // Add your application code
        int dist = calc_dist();
        
        if (dist <= 512){
            rLED=1;
            gLED=0;
            occupied=1;
        } else {
            rLED=0;
            gLED=1;
            occupied=0;
        }
        
        //putch('D');
        //putch('i');
        //printf("stance: %i\n", calc_dist());
        __delay_ms(250);
    }
}

// Definition Of The calc_dist() Function
int calc_dist(void)
{
  unsigned long int distance=0;
  unsigned long int timer=0;
  TMR1=0;
  // Send Trigger Pulse To The Sensor
  trigger=1;
  __delay_us(10);
  trigger=0;
  // Wait For The Echo Pulse From The Sensor
  while(!echo);
  // Turn ON Timer Module
  TMR1ON=1;
  // Wait Until The Pulse Ends
  while(echo);
  // Turn OFF The Timer
  TMR1ON=0;
  // Calculate The Distance Using The Equation
  distance=TMR1/58.82;
  //distance=TMR1/59;
  timer=TMR1;
  
  //printf("%lu", timer);
  return distance;
}

/**
 End of File
*/