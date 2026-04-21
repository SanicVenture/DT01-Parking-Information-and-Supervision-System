# DT01-Parking-Information-and-Supervision-System
This ECE Senior Design Project is intended to sense available parking spots in a garage or lot, and displays the open spots to drivers entering the lot. This repository includes code for a microcontroller, a C# server, and a Flutter application.

## uC Transition
The controller code was transitioned to another microcontroller. The original project was for the PIC18F67J60 which included Ethernet functionality. Documentation for this microcontroller (specifically the TCP/IP stack) was lacking, so it's transitioned to the STM32F446RE microcontroller and W5500 (WIZ850io module) for Ethernet. 