#include "Arduino.h"

String serialMessage = "";

//Max number of characters expected, including delimiters
const int MaxMessageLength = 18;
String results[6];
int intResults[6];


void setup()
{
  Serial.begin(9600);
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

  int counter = 0;
  while ((result = strtok_r(messagePointer, ",", &messagePointer))!= NULL)
  {
    results[counter] = result;
    counter++;
  }
  
  for(int i = 0; i < 6; i++)
  {
    intResults[i] = results[i].toInt();
  }
  
  //Runtime for method
  Serial.println("Time was: " + String(millis() - time));
}
