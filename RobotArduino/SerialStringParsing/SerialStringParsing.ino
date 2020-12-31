#include "Arduino.h"

String serialMessage = "";

//Max number of characters expected, including delimiters
const int MaxMessageLength = 42;


void setup()
{
  Serial.begin(115200);
  Serial.println("Ready to recieve!");
}


void loop()
{
  if ( Serial.available()) 
  {    
    ParseMessage();
  }
}


void ParseMessage()
{
  int time = millis();
  serialMessage = Serial.readStringUntil('\r\n');
  
  char messageArray[MaxMessageLength];
  serialMessage.toCharArray(messageArray, sizeof(messageArray));
  
  char *messagePointer = messageArray;
  char *result;
  
  while ((result = strtok_r(messagePointer, ",", &messagePointer))!= NULL)
  {
    Serial.println(result);
  }
  
  //Runtime for method
  Serial.println(String(millis() - time));
}
