int data;

const int ledPin =  13;

int ledState = LOW;   
unsigned long previousMillis = 0;  
int interval = 1000;

void setup()
{
  Serial.begin(9600);
  pinMode(13, OUTPUT);
}

void loop()
{
  if (Serial.available())
  {
    Serial.write(Serial.read());
    interval = 1000 / Serial.parseInt();
    Serial.flush();
  }

  else
  {
    interval = 1000;
  }

  Blink();
}

void Blink()
{
  unsigned long currentMillis = millis();

  if (currentMillis - previousMillis >= interval) 
  {
    previousMillis = currentMillis;

    if (ledState == LOW) 
    {
      ledState = HIGH;
    } 
    
    else 
    {
      ledState = LOW;
    }

    digitalWrite(ledPin, ledState);
  }
}
