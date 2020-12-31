int data;

void setup()
{
  Serial.begin(9600);
  pinMode(13, OUTPUT);
}

void loop()
{
  if (Serial.available())
  {
    data = Serial.read();

    if (data == 'A')
    {
      digitalWrite(13, HIGH);
    }
    else
    {
      digitalWrite(13, LOW);
    }

    Serial.write(data);
    Serial.flush();
  }
}
