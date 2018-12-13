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
Adafruit_NeoPixel strip = Adafruit_NeoPixel(64, PIN, NEO_GRB + NEO_KHZ800);

#define TEST_MODE 0

// the setup routine runs once when you press reset:
void setup() {                
  // start serial port at 9600 bps:
  Serial.begin(9600);

  strip.begin();
  strip.show(); // Initialize all pixels to 'off'

  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  colorFill(20,0,0);  
}

const byte ports [] = { 0,1,2,3,4,5,6,7 };

// Sends the serial data for the ports
// Sends an eight bit value for each port
// Does not send the value 255 as this is a flag value
// Packet has format 255, 'D', vvvvv..., checksum
void sendData()
{
  byte i;
  byte check = 0;
  // Send the packet start
  //Serial.write(255);
 // Serial.write('D');
 // Serial.write(sizeof(ports));
 Serial.println("---------------------------------");
  for (i=0; i < sizeof(ports); i++)
  {
    Serial.print(i);
    Serial.print(": ");
    Serial.print(analogRead(ports[i]));
    Serial.print(" - ");
    // Get the value and divide to make a single byte
    int data = analogRead(ports[i]) >> 2;
    Serial.println(data);
#if TEST_MODE == 1
    data = i;
#endif
    // Bit stuff, we never send 255 as that is a flag value
    if (data == 255 ) data = 254;
  //  Serial.write(data);
    
    check = check + data;    
  }
 // Serial.write(check);
}

// the loop routine runs over and over again forever:
void loop() {
  if (Serial.available() > 0 ) 
  {
    int command = Serial.read();
    if ( command == 'R' )
      sendData();
   if ( command == 'P')
     receivePanel();
  }
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
      colorFill(0,0,20);
    }
}

// Fill the dots witn one colour
void colorFill(byte r, byte b, byte g) {
  for(uint16_t i=0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, r, b, g);
  } 
  strip.show();
}
