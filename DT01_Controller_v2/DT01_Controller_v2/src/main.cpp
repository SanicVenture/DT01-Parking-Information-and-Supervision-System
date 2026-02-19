/*
 * Senior Design Team 1
 * University of Akron
 * Department of Electrical and Computer Engineering
 * Members: Jacob Dye, Alex Kinch, and Josh Thum
 * Project: Ultrasonic Parking Sensor (Parking Information Supervision System)
 * Objective: To sense vehicles and light a status light to match.
 */

// The following lines are from DT01_Controller/main.c
#include "stm32f4xx_hal.h"  // Include STM32 HAL header
#include <stdint.h>
//#ifdef __cplusplus
#include <Arduino.h>
#include "Ethernet_Generic.h"
//#endif
//#include "../Core/Inc/main.h" // pin definitions and GPIO ports (project-relative path)

//forward declarations
int calc_dist(void);
static void dt01_timer_init(void);
static inline uint32_t dt01_micros(void);
static void dt01_delay_us(uint32_t us);
static void echo_isr(void);

/* TIM5-based microsecond timebase */
static TIM_HandleTypeDef htim5;

/* Echo capture state (updated from ISR) */
static volatile uint32_t echo_t_start = 0;
static volatile uint32_t echo_t_end = 0;
static volatile uint8_t have_rise = 0;
static volatile uint8_t have_fall = 0;

/* Application state (kept across loop calls) */
static uint8_t app_occupied = 0;
static uint8_t app_error = 0;


//define pins
const uint8_t rLED = PA0; // PA0
const uint8_t gLED = PA1; // PA1
const uint8_t trigger = PC1; // PC1
const uint8_t echo = PC0; // PC0 

/* C entry points called from Arduino-compatible C++ wrappers */
void setup(void)
{
  //HAL_Init();
  // SystemClock_Config();

  /* Initialize GPIOs configured by CubeMX */
  //MX_GPIO_Init();
  //HAL_GPIO_Init()

  //intialize GPIO pins using Arduino functions
  pinMode(rLED, OUTPUT);
  pinMode(gLED, OUTPUT);
  pinMode(trigger, OUTPUT);
  pinMode(echo, INPUT);

  //intialize serial for debugging
  Serial.begin(115200);
  Serial.println("Ultrasonic Parking Sensor Starting...");

  /* Initialize TIM5-based microsecond timing and echo ISR */
  dt01_timer_init();

  /* simple LED startup blink */
  //HAL_GPIO_WritePin(rLED_GPIO_Port, rLED_Pin, GPIO_PIN_SET);
  digitalWrite(rLED,1);
  digitalWrite(gLED,1);
  //HAL_GPIO_WritePin(gLED_GPIO_Port, gLED_Pin, GPIO_PIN_SET);
  // replace HAL_Delay with Arduino delay function
  delay(500);
  // HAL_GPIO_WritePin(rLED_GPIO_Port, rLED_Pin, GPIO_PIN_RESET);
  // HAL_GPIO_WritePin(gLED_GPIO_Port, gLED_Pin, GPIO_PIN_RESET);
  digitalWrite(rLED,0);
  digitalWrite(gLED,0);

  app_occupied = 0;
  app_error = 0;
}

void loop(void)
{
  int dist = calc_dist();
  Serial.print("Distance: ");
  Serial.print(dist);
  Serial.println(" cm");

  if (dist <= 100) {
    digitalWrite(rLED,1);
    digitalWrite(gLED,0);
    app_occupied = 1;
    if (app_error == 1) {
      digitalWrite(gLED,1);
    }
  } else {
    digitalWrite(rLED,0);
    digitalWrite(gLED,1);
    app_occupied = 0;
    if (app_error == 1) {
      digitalWrite(rLED,1);
    }
  }

  HAL_Delay(250);
}

// microsecond timing using TIM1 update interrupt and CNT
static void dt01_timer_init(void)
{
  __HAL_RCC_TIM5_CLK_ENABLE();

  htim5.Instance = TIM5;
  htim5.Init.Prescaler = (uint32_t)((SystemCoreClock / 1000000UL) - 1U);
  htim5.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim5.Init.Period = 0xFFFFFFFF;
  htim5.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim5.Init.RepetitionCounter = 0;

  if (HAL_TIM_Base_Init(&htim5) != HAL_OK) {
    app_error = 1;
  }

  // Start TIM5 as a free-running 32-bit microsecond counter
  if (HAL_TIM_Base_Start(&htim5) != HAL_OK) {
    app_error = 1;
  }

  // Attach ISR to echo pin (captures rising and falling edges)
  attachInterrupt(digitalPinToInterrupt(echo), echo_isr, CHANGE);
}

// Simple ISR attached via Arduino attachInterrupt to capture echo timestamps
static void echo_isr(void)
{
  if (digitalRead(echo) == HIGH) {
    echo_t_start = TIM5->CNT;
    have_rise = 1;
    have_fall = 0;
  } else {
    if (have_rise) {
      echo_t_end = TIM5->CNT;
      have_fall = 1;
    }
  }
}

static inline uint32_t dt01_micros(void)
{
  return (uint32_t)TIM5->CNT;
}

static void dt01_delay_us(uint32_t us)
{
  uint32_t start = dt01_micros();
  while ((dt01_micros() - start) < us) {
    __NOP();
  }
}

// Definition Of The calc_dist() Function
int calc_dist(void)
{
  unsigned long distance = 0;
  unsigned long timer = 0;

  // Ensure trigger is low
  // HAL_GPIO_WritePin(trigger_GPIO_Port, trigger_Pin, GPIO_PIN_RESET);
  digitalWrite(trigger,0);
  //dt01_delay_us(2);
  delayMicroseconds(2);

  // Send Trigger Pulse To The Sensor (10 us)
  digitalWrite(trigger,1);
  delayMicroseconds(10);
  digitalWrite(trigger,0);

  // Wait for echo rising edge (with timeout)
  uint32_t start_wait = dt01_micros();
  while (digitalRead(echo) == LOW) {
    if ((dt01_micros() - start_wait) > 30000UL) { // 30 ms timeout
      return 0; // timeout -> no object
    }
  }

  // measure pulse width
  uint32_t t_start = dt01_micros();
  while (digitalRead(echo) == HIGH) {
    if ((dt01_micros() - t_start) > 30000UL) { // 30 ms safety
      break;
    }
  }
  uint32_t t_end = dt01_micros();

  timer = (t_end - t_start);
  distance = (unsigned long)(timer / 58.82);

  return (int)distance;
}