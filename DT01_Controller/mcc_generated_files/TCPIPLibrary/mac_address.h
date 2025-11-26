/* 
 * File:   mac_address.h
 * Author: alext
 *
 * Created on November 25, 2025, 7:17 PM
 */

#ifndef MAC_ADDRESS_H
#define	MAC_ADDRESS_H

#ifdef	__cplusplus
extern "C" {
#endif

#include <stdint.h>

typedef union {
    uint8_t mac_array[6];
    struct {
        uint8_t byte1;
        uint8_t byte2;
        uint8_t byte3;
        uint8_t byte4;
        uint8_t byte5;
        uint8_t byte6;
    } s;
} mac48Address_t;



#ifdef	__cplusplus
}
#endif

#endif	/* MAC_ADDRESS_H */

