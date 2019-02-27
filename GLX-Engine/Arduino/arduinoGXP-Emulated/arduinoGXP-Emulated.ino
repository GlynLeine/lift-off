/*
  Send data to GXP

  created 10 Feb 2019
  modified 24 Feb 2019
  by Eusebiu Mihail Buga

  For the Arduino Leonardo, Micro or Due

  The circuit:
  - as long as it sends something to the serial port it can even be a gravity gun built with scavenged MOSFETs


*/

int analogs = 0;
int digitals = 0;
int analogID = 0;
int digitalID = 0;
int ID;

bool initialiseRun = true;
bool connectedToEngine = false;

void setup()
{
  // open the serial port:
  Serial.begin(9600);
  Serial.setTimeout(5000);

  pinMode(2, INPUT_PULLUP);
  pinMode(3, INPUT_PULLUP);
  pinMode(4, INPUT_PULLUP);
  pinMode(5, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);

  pinMode(8, OUTPUT);
  pinMode(9, OUTPUT);
  pinMode(10, OUTPUT);

  while (!Serial)
  {
    ;
  }

  randomSeed(analogRead(0));
  ID = random(32767);

  loop();
  initialiseRun = false;
}

void loop()
{
  String message = RetrieveMessage();
  if (CheckHandshake(message) || initialiseRun)
  {
    //
    if (initialiseRun)
    {
      analogs++;
    }
    String toSend = "ANALOG";
    toSend.concat(analogID++);
    toSend.concat("=");

    float analogValue = 0.5;
    analogValue = (float)((int)(analogValue * 10)) / 10.f;

    toSend.concat(analogValue);
    Serial.println(toSend);
    //

    //
    if (initialiseRun)
    {
      digitals++;
    }
    toSend = "DIGITAL";
    toSend.concat(digitalID++);
    toSend.concat("=");

    bool digitalValue = false;

    toSend.concat(!digitalValue);
    Serial.println(toSend);
    //
  }
}

void SendAnalogData(int pin)
{
  if (initialiseRun)
  {
    analogs++;
  }
  String toSend = "ANALOG";
  toSend.concat(analogID++);
  toSend.concat("=");

  float analogValue = (analogRead(pin) / 511.5) - 1;
  analogValue = (float)((int)(analogValue * 10)) / 10.f;

  toSend.concat(analogValue);
  Serial.println(toSend);

}

void SendDigitalData(int pin)
{
  if (initialiseRun)
  {
    digitals++;
  }
  String toSend = "DIGITAL";
  toSend.concat(digitalID++);
  toSend.concat("=");

  bool digitalValue = digitalRead(pin);
  //  if (digitalValue == false)
  //  {
  //    digitalWrite(pin + 6, HIGH);
  //  }
  //  else
  //  {
  //    digitalWrite(pin + 6, LOW);
  //  }

  toSend.concat(!digitalValue);
  Serial.println(toSend);
}

String RetrieveMessage()
{
  digitalID = 0;
  analogID = 0;

  String message;
  while (Serial.available() > 0)
  {
    char c = Serial.read();
    message.concat(c);
  }
  return message;
}

bool CheckHandshake(String& message)
{
  if (message.indexOf("GIVE HANDSHAKE") >= 0)
  {
    Serial.println("HANDSHAKE");
    Serial.print("ID");
    Serial.println(ID);
    Serial.print("ANALOGS");
    Serial.println(analogs);
    Serial.print("DIGITALS");
    Serial.println(digitals);
    connectedToEngine = true;
    return true;
  }
  else
  {
    if (message.indexOf("SEND") >= 0)
    {
      return true;
    }
  }
  return false;
}
