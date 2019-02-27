/*
  Interface for the serial port and GXP
  
  created 10 Feb 2019
  modified 24 Feb 2019
  by Eusebiu Mihail Buga
  

  The class auto-detects the serial port of whatever arduino you use as long as it sends back the "HANDSHAKE" message, so you
  don't have to worry about port name changing every time you boot your PC or restart the Arduino

  Use this to your heart's content
  As all code in existence, this class is garbage, if you want to modify it or make it prettier, by all means do so

  If you don't have the "Ports" library referenced this is how you do it
    -Right click References on your solution manager
    -Click "add reference"
    -Search for "System"
    -Tick the box
    -Enjoy a library used for embedded stuff made by web developers
*/


using System.IO.Ports;
using System;

public class ArduinoInterface
{
    static SerialPort port = null;
    private String[] Ports = SerialPort.GetPortNames();
    private bool found = false;
    private String _message;
    private static int _parameterSize = 8;//number of parameters, i think 8 is plenty
    private String[] _parameters = new String[_parameterSize];


    /* !!IMPORTANT!!
    ****If you use an arduino Uno, Nano, or any not USB-native chips, set this to true
    */
    private const bool _useUno = false;

    public ArduinoInterface()
    {
        Console.WriteLine("blah");
        Initialise();
        Console.WriteLine("bleh");
    }

    private void Initialise()
    {
        //initialize parameter array
        Search();
    }

    private void Search()
    {
        port = null;

        for (int i = 0; i < _parameterSize; i++)
        {
            _parameters[i] = "0";
        }

        while (!found)
        {
            Ports = SerialPort.GetPortNames();
            foreach (String portName in Ports)
            {
                port = new SerialPort(portName);

                port.BaudRate = 9600;
                port.ReadTimeout = 1000000;


                if (_useUno == false)
                {
                    port.RtsEnable = true;
                    port.DtrEnable = true;
                }
                else
                {
                    port.RtsEnable = false;
                    port.DtrEnable = false;
                }

                if (port.IsOpen)
                {
                    port.Close();
                    try { port.Open(); }
                    catch (System.IO.IOException e) { continue; }
                }
                else
                {
                    try { port.Open(); }
                    catch (System.IO.IOException e) { continue; }
                }

                port.DiscardOutBuffer();
                port.DiscardInBuffer();

                Console.WriteLine("Send Data pls");
                SendString("GIVE HANDSHAKE");

                Console.WriteLine("Gimme Data pls");
                String Accept = null;
                bool accepted = false;

                while (!accepted)
                    try
                    {
                        Accept = port.ReadLine();
                        accepted = true;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("FUUUUUUUU");
                    }

                Console.WriteLine(Accept);
                if (Accept.Contains("HANDSHAKE"))
                {
                    found = true;
                    port.Write("FOUND");
                    break;
                }

                port.DiscardInBuffer();
                Console.WriteLine("blih");
            }
            Console.WriteLine("bloh");
        }

        port.DiscardInBuffer();

        System.Console.WriteLine("Initializing port");
    }

    public String GetMessage()
    {
        getString();
        return _message;
    }

    /// <summary>
    /// Returns a string representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>String</returns>
    public String GetStringParameter(int parameter)
    {
        getString();
        return _parameters[parameter];
    }

    /// <summary>
    /// Returns an integer representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>Int</returns>
    public int GetIntParameter(int parameter)
    {
        getString();
        return Int32.Parse(_parameters[parameter]);
    }

    /// <summary>
    /// Returns a float representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>float</returns>
    public float GetFloatParameter(int parameter)
    {
        getString();
        return float.Parse(_parameters[parameter]);
    }

    private void SendString(string send)
    {
        if (port.IsOpen)
            try
            {
                port.Write(send);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("bitch is non existent");
                found = false;
                Search();
                SendString(send);
                GLXEngine.Time.previousTime = GLXEngine.Time.time;
            }
        else
        {
            Console.WriteLine("bitch is closed");
            found = false;
            Search();
            SendString(send);
            GLXEngine.Time.previousTime = GLXEngine.Time.time;
        }
    }

    private void getString()
    {

        //Doesn't really matter what you send
        SendString("SEND");

        try { _message = port.ReadLine(); }
        catch (System.IO.IOException e) { _message = ""; }

        //Discard the buffer
        //if (port.BytesToRead > 100)
        //{
        //    port.DiscardInBuffer();
        //    port.ReadLine();
        //}


        //Split the message into parameters
        if (_message.Contains("HANDSHAKE"))
        {
            _message = _message.Remove(0, 10);
            _message.Trim();
            _parameters = _message.Split(' ');
        }
    }


    //Deconstructors in C# ??? is this even legal
    ~ArduinoInterface()
    {
        port.Close();
    }
}