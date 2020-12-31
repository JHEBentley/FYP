/*
 * Reads the serial feed and parses ints, then writes this in as the servo motor position.
 * Can be used in cnjunction with the Arduino comms script in Unity.
 * Note, the motor can only move between positions 0-90. * 
 */



#include <Servo.h>;

Servo servo;
int data;
int oldData;

void setup()
{
  Serial.begin(9600);
  servo.attach(7);

  data = 0;
  oldData = 0;
  
  Serial.println("Arduino is ready");
}

void loop() 
{
  //If there's new serial data available, assign it as the data var and flush the channel to prevent contamination of serial feeds
  if (Serial.available())
  {
    data = Serial.parseInt();
    Serial.flush();
  }

  else
  {
    data = 90;
  }

  //If we have recieved new data, send it back to the monitor
  if (data != oldData)
  {
    oldData = data;
    Serial.println(data);
  }
  
  servo.write(data);
}
