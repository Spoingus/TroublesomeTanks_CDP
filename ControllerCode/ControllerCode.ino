/*
  Controller Controller
 
  Sends the sensor information up to the game
  
 */
 
 
#include <Adafruit_NeoPixel.h>

#define PIN 7

// Parameter 1 = number of pixels in strip
// Parameter 2 = pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
Adafruit_NeoPixel strip = Adafruit_NeoPixel(61, PIN, NEO_GRB + NEO_KHZ800);

//#define TEST_MODE 0

// the setup routine runs once when you press reset:
void setup() {                
  // start serial port at 9600 bps:
  Serial.begin(9600);

  strip.begin();
  strip.show(); // Initialize all pixels to 'off'

  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  initialiseAnimation();
}

const byte ports [] = { 0,1,2,3,4,5,6 };

// Sends the serial data for the ports
// Sends an eight bit value for each port
// Does not send the value 255 as this is a flag value
// Packet has format 255, 'D', vvvvv..., checksum
void sendData()
{
  byte i;
  byte check = 0;
  // Send the packet start
  Serial.write(255);
  Serial.write('D');
  Serial.write(sizeof(ports));
 //Serial.println("---------------------------------");
  for (i=0; i < sizeof(ports); i++)
  {
   /* Serial.print(i);
    Serial.print(": ");
    Serial.print(analogRead(ports[i]));
    Serial.print(" - ");
    // Get the value and divide to make a single byte*/
  //  int data = analogRead(ports[i]) >> 2;
    //Serial.println(data);
    byte id = IdentifyConnection(i);
#if TEST_MODE == 1
 //   data = i;
#endif
    // Bit stuff, we never send 255 as that is a flag value
 //   if (data == 255 ) data = 254;
    Serial.write(id);
  //Serial.print("bit shift");
 // Serial.println(id);
    
    check = check + id;    
  }
  Serial.write(check);
}

// the loop routine runs over and over again forever:
void loop() {
  if (Serial.available() > 0 ) 
  {
    int command = Serial.read();
    if ( command == 'R' )
    {
      sendData();
    }
   if ( command == 'P')
   {
     receivePanel();
   }
    if( command == 'I')
    {
      identifyDevice();
    }
  }
}
void identifyDevice()
{  
  Serial.write("Tankontroller");
}

byte IdentifyConnection(byte port) {
  int id = 0;
  int data = analogRead(ports[port]);
  int tolerance = 10;
  //Serial.print("Port: ");
  //Serial.print(port);
  //Serial.print(" is connected to ");
  if(IsNearly(data, 1021, tolerance)) {    
  //  Serial.println("nothing");
    return 0;
  }
  else if(IsNearly(data, 932, tolerance)) {    
  //  Serial.println("charge - unpressed");
    return 1;
  }
  else if(IsNearly(data, 181, tolerance)) {    
  //  Serial.println("charge - pressed");
    return 2;
  }
  else if(IsNearly(data, 608, tolerance)) {    
  //  Serial.println("right forward - unpressed");
    return 3;
  }
  else if(IsNearly(data, 7, tolerance)) {    
  //  Serial.println("right forward - pressed");
    return 4;
  }
  else if(IsNearly(data, 699, tolerance)) {    
   // Serial.println("right backward - unpressed");
    return 5;
  }
  else if(IsNearly(data, 21, tolerance)) {    
  //  Serial.println("right backward - pressed");
    return 6;
  }
  else if(IsNearly(data, 785, tolerance)) {    
   // Serial.println("left forward - unpressed");
    return 7;
  }
  else if(IsNearly(data, 45, tolerance)) {    
    //Serial.println("left forward - pressed");
    return 8;
  }
  else if(IsNearly(data, 836, tolerance)) {    
   // Serial.println("left backward - unpressed");
    return 9;
  }
  else if(IsNearly(data, 93, tolerance)) {    
   // Serial.println("left backward - pressed");
    return 10;
  }
  else if(IsNearly(data, 959, tolerance)) {    
   // Serial.println("gun fire - unpressed");
    return 11;
  }
  else if(IsNearly(data, 327, tolerance)) {    
   // Serial.println("gun fire - pressed");
    return 12;
  }
  else if(IsNearly(data, 1002, tolerance)) {    
   // Serial.println("gun right - unpressed");
    return 13;
  }
  else if(IsNearly(data, 511, tolerance)) {    
   // Serial.println("gun right - pressed");
    return 14;
  }
  else if(IsNearly(data, 978, tolerance)) {    
   // Serial.println("gun left - unpressed");
    return 15;
  }
  else if(IsNearly(data, 402, tolerance)) {    
   // Serial.println("gun left - pressed");
    return 16;
  }
}

bool IsNearly(int data, int target, int tolerance) {
  int lowerBound = target - tolerance;
  int upperBound = target + tolerance;
  if (data > lowerBound && data < upperBound) {
    return true;
  }
  return false;
   
}

byte GetCh()
{
  int ch;
  do
  {
    ch = Serial.read();
  } while ( ch < 0);
  
  return (byte) ch;
}

void receivePanel()
{
    byte pixelCount = GetCh();  
    byte check =0;
    int i;
    for ( i=0; i < pixelCount; i++)
    {
      byte r = GetCh();
      check += r;
      byte g = GetCh();
      check += g;
      byte b = GetCh();
      check += b;
      strip.setPixelColor(i, r, b, g);
    }
    
    byte receivedCheck = GetCh();
    
    if (check == receivedCheck)
    {    
      strip.show();
    }
    else {
    //  colorFill(20,0,0);
    }
}

// Fill the dots witn one colour
void initialiseAnimation() {
  colorFill(20, 0, 0);
  delay(500);
  colorFill(20, 0, 20);
  delay(500);
  colorFill(20, 20, 0);
  delay(500);
  colorFill(0, 0, 0);
}

// Fill the dots witn one colour
void colorFill(byte r, byte b, byte g) {
  for(uint16_t i=0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, r, b, g);
  } 
  strip.show();
}
