using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

//-----------------------------------------------------------------------------------------------
// Hacktroller.cs
// This is a class that interfaces with the Hacktroller device. The Hacktroller is a custom
// device that is used to control a tank. It has a number of pins that can be connected to
// switches and buttons. The class reads the state of the pins and decodes them into a
// ControllerState. The class also has a method to set the colour of the LEDs on the device.
//-----------------------------------------------------------------------------------------------
namespace Tankontroller.Controller
{
    /*      R1     R2    R3     Off   On
            100k   47k   0      323   0
            5k6    350   53
            10k    369   91
            15k    389   132
            18k    400   154
            33k    450   252*/

    //-----------------------------------------------------------------------------------------------
    // ControllerColor
    //
    // This struct is used to store the colour of the LEDs on the Hacktroller device.
    //-----------------------------------------------------------------------------------------------
    public struct ControllerColor
    {
        public byte R;
        public byte G;
        public byte B;

        public ControllerColor(byte inR, byte inG, byte inB)
        {
            R = inR;
            G = inG;
            B = inB;
        }
    }

    //-----------------------------------------------------------------------------------------------
    // ControllerState
    //
    // This enum is used to store the state of the Hacktroller device.
    //-----------------------------------------------------------------------------------------------
    public enum ControllerState
    {
        LEFT_TRACK_FORWARDS,
        LEFT_TRACK_FORWARDS_PRESSED,

        LEFT_TRACK_BACKWARDS,
        LEFT_TRACK_BACKWARDS_PRESSED,

        RIGHT_TRACK_FORWARDS,
        RIGHT_TRACK_FORWARDS_PRESSED,

        RIGHT_TRACK_BACKWARDS,
        RIGHT_TRACK_BACKWARDS_PRESSED,

        TURRET_LEFT,
        TURRET_LEFT_PRESSED,

        TURRET_RIGHT,
        TURRET_RIGHT_PRESSED,

        CHARGE,
        CHARGE_PRESSED,

        FIRE,
        FIRE_PRESSED,

        NO_MATCH,
        NOT_CONNECTED
    }

    //-----------------------------------------------------------------------------------------------
    // stateMap
    //
    // This struct is used to map the reading of a pin to a ControllerState.
    // It contains the following:
    // - A ControllerState to store the state of the pin
    // - An int to store the reading of the pin
    //-----------------------------------------------------------------------------------------------
    public struct stateMap
    {
        public ControllerState Result;
        public int Reading;
        public stateMap(ControllerState decodedState, int readingValue)
        {
            Result = decodedState;
            Reading = readingValue;
        }
    }

    //-----------------------------------------------------------------------------------------------
    // Hacktroller
    //
    // This class is used to interface with the Hacktroller device.
    // It contains the following:
    // - A SerialPort object to communicate with the device
    // - A Random object to generate random numbers
    // - A byte array to store the command to read the state of the pins
    // - A byte array to store the command to set the colour of the LEDs
    // - A byte array to store the frame buffer
    // - An array of PortState objects to store the state of the pins
    // - A static array of stateMap objects to map the pin readings to ControllerStates
    // - A static array of PinState objects to store the state of the pins
    // - A static int to store the tolerance for the pin readings
    //-----------------------------------------------------------------------------------------------
    public class Hacktroller
    {
        SerialPort port;
        public string PortName { get { return port.PortName; } }

        static int numPins = 7;
        static byte[] SetColorCommand = new byte[] { (byte)'P', 0 };
        static byte[] SetIDCommand = new byte[] { (byte)'U', (byte)' ' };
        static byte[] GetPortCommand = new byte[] { (byte)'R' };

        static byte[] frameBuffer = new byte[61 * 3];

        static int tolerance = 0;

        ControllerState[] portStates = new ControllerState[numPins];

        static stateMap[] stateMapping = new stateMap[]
        {
            new stateMap(ControllerState.RIGHT_TRACK_FORWARDS, 3),
            new stateMap(ControllerState.RIGHT_TRACK_FORWARDS_PRESSED, 4),

            new stateMap(ControllerState.RIGHT_TRACK_BACKWARDS, 5),
            new stateMap(ControllerState.RIGHT_TRACK_BACKWARDS_PRESSED, 6),

            new stateMap(ControllerState.LEFT_TRACK_FORWARDS, 7),
            new stateMap(ControllerState.LEFT_TRACK_FORWARDS_PRESSED, 8),

            new stateMap(ControllerState.LEFT_TRACK_BACKWARDS, 9),
            new stateMap(ControllerState.LEFT_TRACK_BACKWARDS_PRESSED, 10),

            new stateMap(ControllerState.CHARGE, 1),
            new stateMap(ControllerState.CHARGE_PRESSED, 2),

            new stateMap(ControllerState.FIRE, 11),
            new stateMap(ControllerState.FIRE_PRESSED, 12),

            new stateMap(ControllerState.TURRET_LEFT, 13),
            new stateMap(ControllerState.TURRET_LEFT_PRESSED, 14),

            new stateMap(ControllerState.TURRET_RIGHT, 15),
            new stateMap(ControllerState.TURRET_RIGHT_PRESSED, 16),

            new stateMap(ControllerState.NOT_CONNECTED, 0)
        };

        ControllerState DecodeState(int reading)
        {
            foreach (stateMap s in stateMapping)
            {
                float diff = Math.Abs(s.Reading - reading);
                if (diff <= tolerance)
                    return s.Result;
            }
            return ControllerState.NO_MATCH;
        }

        public Hacktroller(string portName)
        {
            port = new SerialPort(portName, 19200);//, Parity.None, 8, StopBits.One);
            port.Open();

            port.DtrEnable = true;
            port.ReadTimeout = 10;
            port.WriteTimeout = 10;
        }

        public Hacktroller(SerialPort pPort)
        {
            port = pPort;

            for (int i = 0; i < frameBuffer.Length; i++)
            {
                frameBuffer[i] = 20;
            }
        }

        public void SetID(string pID)
        {
            if (pID.Length > 9)
            {
                pID = pID.Substring(0, 10);
                try
                {
                    port.DiscardInBuffer();
                    port.Write(SetIDCommand, 0, 2);
                    port.Write(pID);
                }
                catch
                { }
            }
        }

        public ControllerState[] GetPorts()
        {
            try
            {
                port.DiscardInBuffer();
                port.Write(GetPortCommand, 0, 1);

                System.Threading.Thread.Sleep(10);

                if (port.BytesToRead > 0)
                {
                    // The comms starts with 0xff 0xff
                    var b = port.ReadByte();
                    if (b != 0xff) return null;

                    b = port.ReadByte();
                    if (b != 'D') return null;

                    byte[] buffer = new byte[numPins + 1];

                    port.Read(buffer, 0, buffer.Length);

                    for (int i = 0; i < numPins; i++)
                    {
                        int reading = buffer[i + 1];
                        ControllerState readingState = DecodeState(reading);
                        portStates[i] = readingState;
                    }
                }
            }
            catch
            {
                return null;
            }
            return portStates;
        }


        public async Task<bool> SetColor(Dictionary<byte, ControllerColor> colourData)
        {
            byte[] colourWriteCommand = SetColorCommand;
            byte[] writeColour = new byte[4];

            // List can't be larger than 255 as it is sent as a byte (There are only 10 LEDs on a controller anyways)
            if (colourData.Count > 254)
                return false;

            colourWriteCommand[1] = (byte)colourData.Count;

            // Write the command
            await port.BaseStream.WriteAsync(colourWriteCommand, 0, 2);

            byte check = 0;
            foreach (KeyValuePair<byte, ControllerColor> data in colourData)
            {
                // HACK think this is RBG instead of RGB so have swapped subscripts around
                writeColour[0] = data.Key;
                check += data.Value.R;
                writeColour[1] = data.Value.R;
                check += data.Value.G;
                writeColour[3] = data.Value.G;
                check += data.Value.B;
                writeColour[2] = data.Value.B;
                await port.BaseStream.WriteAsync(writeColour, 0, 4);
            }

            // Write out the checksum
            writeColour[0] = check;
            await port.BaseStream.WriteAsync(writeColour, 0, 1);
            return true;
        }
    }
}
