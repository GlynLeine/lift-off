/*
  Send data to GXP

  created 10 Feb 2019
  modified 24 Feb 2019
  by Eusebiu Mihail Buga
  
  For the Arduino Leonardo, Micro or Due
  
  The circuit:
  - as long as it sends something to the serial port it can even be a gravity gun built with scavenged MOSFETs


*/

void setup() {
  // open the serial port:
  Serial.begin(9600);

  //Used for native USB boards, don't have to use this with an Uno or Nano or the like
  while(!Serial){
    ;
  }
}

void loop()
{
  String message;
  while(Serial.available() > 0)
  {
    char c = Serial.read();
    message.concat(c);
  }
  
  if(message.indexOf("GIVE HANDSHAKE") >= 0)
  {
    String s = "HANDSHAKE";
    Serial.println(s);
  }
  
  if(message.indexOf("SEND") >= 0)
  {
    String s;
    //Just concatenate everything you want to send, with a space in between
    s.concat(analogRead(A0));
    s.concat(" ");
    s.concat(analogRead(A1));
    //After everything is done just send the data
    Serial.println(s);
  }

  //Delay of 1ms is fine, any delay is fine really, you just need it because the Arduino is faster than GXP or something like that ¯\_(ツ)_/¯
  //delay(1);
  //Nah we don't need no delay anymore
 } 


void Flush(){
  while(Serial.available())
   char c=Serial.read();
}
