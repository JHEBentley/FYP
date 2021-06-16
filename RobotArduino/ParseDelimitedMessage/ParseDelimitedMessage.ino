/*
 * Reads the serial feed and parses ints, then writes this in as the servo motor position.
 * Does this for a string of values with "," delimiters.
 * Can be used in conjunction with the Arduino comms script in Unity.
 */

#include <Servo.h>;

//Servo stuff
Servo servos[6];
int data[] = {90,90,90,90,90,90};
int oldData[] = {90,90,90,90,90,90};

//Parsing stuff
String serialMessage = "";
//Max number of characters expected, including delimiters
const int MaxMessageLength = 24;
String results[6];
int intResults[6];

void setup()
{
  Serial.begin(9600);

  //Attatch the 6 servos to pins 6 - 12
  for(int i = 0; i < 6; i++)
  {
      servos[i].attach(i + 6);
  }
  
  Serial.println("Arduino is ready");
}

void loop() 
{
  //If there's new serial data available, assign it as the data var and flush the channel to prevent contamination of serial feeds
  if (Serial.available())
  {
    ParseMessage();
    UpdateArray(data, 6, intResults);
    Serial.flush();
  }

  else
  {/*
    for(int i = 0; i < 6; i++)
    {
          data[i] = 90;
    }*/
  }

  //If we have recieved new data, send it back to the monitor
  for(int i = 0; i < 6; i++)
  {
    if (data[i] != oldData[i])
    {
      oldData[i] = data[i];
      Serial.print(i);
      Serial.print(" is at: ");
      Serial.println(data[i]);
    }
  }

  for(int i = 0; i < 6; i++)
  {
    servos[i].write(data[i]);
  }
}

void UpdateArray(int _array[], int _length, int newArray[])
{
  for(int i = 0; i < _length; i++)
  {
     _array[i] = newArray[i];
  }
}

void ParseMessage()
{
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
}
