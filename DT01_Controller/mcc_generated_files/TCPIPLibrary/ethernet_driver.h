/* 
 * File:   ethernet_driver.h
 * Author: alext
 *
 * Created on November 25, 2025, 12:41 AM
 */

#ifndef ETHERNET_DRIVER_H
#define	ETHERNET_DRIVER_H

#include <xc.h>
#include <stdbool.h>
#include <stdint.h>
#include "../eth.h"
// no Assumes mac_address.h is in the parent folder mcc_generated_files/
#include "mac_address.h" 

// ---------------------------------------------------------
// 1. Global Variable Declarations 
// ---------------------------------------------------------
extern mac48Address_t hostMacAddress;
extern const mac48Address_t broadcastMAC;

// ---------------------------------------------------------
// 2. Initialization Mapping
// ---------------------------------------------------------
#define ETH_Init                ETH_Initialize

// ---------------------------------------------------------
// 3. Direct Hardware Implementation (PIC18F67J60)
// ---------------------------------------------------------

static inline bool ETH_packetReady(void) {
    return EIRbits.PKTIF; 
}

static inline uint8_t ETH_Read8(void) {
    return EDATA;
}

static inline void ETH_Write8(uint8_t data) {
    EDATA = data;
}

static inline uint16_t ETH_Read16(void) {
    uint16_t res = EDATA;
    res |= ((uint16_t)EDATA << 8);
    return res;
}

static inline void ETH_Write16(uint16_t data) {
    EDATA = (uint8_t)data;
    EDATA = (uint8_t)(data >> 8);
}

static inline uint32_t ETH_Read32(void) {
    uint32_t res = ETH_Read16();
    res |= ((uint32_t)ETH_Read16() << 16);
    return res;
}

static inline void ETH_Write32(uint32_t data) {
    ETH_Write16((uint16_t)data);
    ETH_Write16((uint16_t)(data >> 16));
}

static inline void ETH_WriteString(const char *string) {
    while (*string != '\0') {
        ETH_Write8(*string++);
    }
}

static inline uint16_t ETH_ReadBlock(void *dest, uint16_t count) {
    uint8_t *d = (uint8_t *)dest;
    uint16_t actual = count;
    while (count--) {
        *d++ = EDATA;
    }
    return actual; 
}

static inline void ETH_WriteBlock(const void *src, uint16_t count) {
    const uint8_t *s = (const uint8_t *)src;
    while (count--) {
        EDATA = *s++;
    }
}

static inline void ETH_SetReadPtr(uint16_t addr) {
    ERDPTL = (uint8_t)addr;
    ERDPTH = (uint8_t)(addr >> 8);
}

static inline uint16_t ETH_GetReadPtr(void) {
    return ((uint16_t)ERDPTH << 8) | ERDPTL;
}

static inline void ETH_SetWritePtr(uint16_t addr) {
    EWRPTL = (uint8_t)addr;
    EWRPTH = (uint8_t)(addr >> 8);
}

static inline uint16_t ETH_GetWritePtr(void) {
    return ((uint16_t)EWRPTH << 8) | EWRPTL;
}

static inline void ETH_SaveRDPT(void) { }
static inline uint16_t ETH_GetRxByteCount(void) { return 0; }
static inline void ETH_SetRxByteCount(uint16_t count) { }
static inline void ETH_ResetByteCount(void) { }
static inline uint16_t ETH_GetByteCount(void) { return 0; }

// ---------------------------------------------------------
// 4. Control Functions
// ---------------------------------------------------------

static inline void ETH_EventHandler(void) {
    if (EIRbits.TXERIF) { 
        ECON1bits.TXRTS = 0; 
        EIRbits.TXERIF = 0; 
    }
}

static inline void ETH_NextPacketUpdate(void) {
    ECON2bits.PKTDEC = 1; 
}

static inline void ETH_Flush(void) { }

static inline void ETH_Dump(uint16_t count) {
    while(count--) {
        volatile uint8_t dummy = EDATA;
        (void)dummy;
    }
}

static inline uint16_t ETH_TxComputeChecksum(uint16_t len, uint16_t seed, uint16_t offset) { return 0; }
static inline uint16_t ETH_RxComputeChecksum(uint16_t len, uint16_t seed) { return 0; }

static inline void ETH_Insert(char *src, uint16_t count, uint16_t offset) { }

static inline int ETH_Copy(uint16_t count) {
    return 0; 
}

static inline bool ETH_CheckLinkUp(void) {
    return true; 
}

static inline int ETH_Send(void) {
    ECON1bits.TXRTS = 1; 
    return 0; 
}

static inline int ETH_WriteStart(const void* destMac, uint16_t type) {
    return 0; 
}

static inline void ETH_GetMAC(uint8_t *mac) {
    mac[0] = MAADR1;
    mac[1] = MAADR2;
    mac[2] = MAADR3;
    mac[3] = MAADR4;
    mac[4] = MAADR5;
    mac[5] = MAADR6;
}

#endif	/* ETHERNET_DRIVER_H */
