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

using GLXEngine;
using System.IO.Ports;
using System.Collections.Generic;
using System.Globalization;
using System;

public class ArduinoInterface
{
    private static String[] m_ports;
    private static List<int> m_ignoreList = null;

    private SerialPort m_port = null;
    protected int m_ID = -1;

    private bool m_found = false;
    private bool m_closed = true;
    private String m_message;

    private List<String> m_parameters = new List<string>();
    protected List<float> m_analogs = new List<float>();
    protected List<bool> m_digitals = new List<bool>();

    public bool found { get { return m_found; } }
    public bool closed { get { return m_closed; } }

    /* !!IMPORTANT!!
    ****If you use an arduino Uno, Nano, or any not USB-native chips, set this to true
    */
    private bool m_useUno = false;

    public ArduinoInterface(bool a_search = false, bool a_persistent = false)
    {
        if (m_ignoreList == null)
            m_ignoreList = new List<int>();

        if (a_search)
            Search(a_persistent);
    }

    public bool Search(bool a_persistent = false)
    {
        m_found = false;
        m_port = null;

        for (int i = 0; !m_found; i++)
        {
            Console.WriteLine("Searching for connected ports.");
            m_ports = SerialPort.GetPortNames();
            foreach (String portName in m_ports)
            {
                m_port = new SerialPort(portName);

                m_port.BaudRate = 9600;
                m_port.ReadTimeout = 500;

                if (m_useUno == false)
                {
                    m_port.RtsEnable = true;
                    m_port.DtrEnable = true;
                }
                else
                {
                    m_port.RtsEnable = false;
                    m_port.DtrEnable = false;
                }

                if (m_port.IsOpen)
                {
                    m_port.Close();
                    try { m_port.Open(); }
                    catch (System.IO.IOException e) { continue; }
                    //catch (System.UnauthorizedAccessException e) { continue; }
                }
                else
                {
                    try { m_port.Open(); }
                    catch (System.IO.IOException e) { continue; }
                    //catch (System.UnauthorizedAccessException e) { continue; }
                }

                m_port.DiscardOutBuffer();
                m_port.DiscardInBuffer();

                Console.WriteLine("Requesting handshake.");
                SendString("GIVE HANDSHAKE");

                String[] handshake = null;
                bool accepted = false;

                while (!accepted)
                    try
                    {
                        handshake = m_port.ReadExisting().Split('\n');
                        accepted = true;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("Waiting for response...");
                    }

                foreach (String message in handshake)
                    Console.WriteLine(message);

                if (handshake[0].Contains("HANDSHAKE"))
                {
                    bool fail = false;

                    for (int j = 1; j < handshake.Length; j++)
                    {
                        String handshakeData = handshake[j];
                        if (handshakeData.StartsWith("ID"))
                        {
                            handshakeData = handshakeData.Remove(0, 2);
                            int ID = int.Parse(handshakeData);
                            if (!m_ignoreList.Contains(m_ID))
                            {
                                m_ID = ID;
                                m_ignoreList.Add(ID);
                            }
                            else
                                fail = true;
                        }
                        else if (handshakeData.StartsWith("ANALOGS"))
                        {
                            handshakeData = handshakeData.Remove(0, 7);
                            int analogs = int.Parse(handshakeData);
                            for (int k = 0; k < analogs; k++)
                            {
                                m_analogs.Add(0f);
                            }
                        }
                        else if (handshakeData.StartsWith("DIGITALS"))
                        {
                            handshakeData = handshakeData.Remove(0, 8);
                            int digitals = int.Parse(handshakeData);
                            for (int k = 0; k < digitals; k++)
                            {
                                m_digitals.Add(false);
                            }
                        }
                    }

                    if (!fail)
                    {
                        m_closed = false;
                        m_found = true;
                        m_port.Write("FOUND");

                        Console.WriteLine("Found controller on port: " + m_port.PortName);
                        Console.WriteLine(m_analogs.Count + m_digitals.Count + " parameters found.");
                        Console.WriteLine("Of which " + m_analogs.Count + " analog.");
                        Console.WriteLine("And " + m_digitals.Count + " digital.");

                        break;
                    }
                }

                m_port.DiscardInBuffer();
            }

            if (!a_persistent && i >= 1)
                return false;
            if (i % 2 == 0)
                m_useUno = false;
            else
                m_useUno = true;
        }

        m_port.DiscardInBuffer();
        m_port.DiscardOutBuffer();

        Console.WriteLine("Found port");
        Time.previousTime = Time.time;
        return true;
    }

    public String GetMessage()
    {
        if (GetData())
            return m_message;
        else return null;
    }

    /// <summary>
    /// Returns a float representing the specific analog parameter from the Arduino
    /// </summary>
    /// <param name="a_analogID">
    /// Specify which analog parameter to return</param>
    /// <returns>float</returns>
    public float GetAnalogParameter(int a_analogID)
    {
        if (GetData())
            return m_analogs[a_analogID];
        else return 0;
    }

    /// <summary>
    /// Returns a boolean representing the specific digital parameter from the Arduino
    /// </summary>
    /// <param name="a_digitalID">
    /// Specify which digital parameter to return</param>
    /// <returns>float</returns>
    public bool GetDigitalParameter(int a_digitalID)
    {
        if (GetData())
            return m_digitals[a_digitalID];
        else return false;
    }

    /// <summary>
    /// Returns a string representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>String</returns>
    public String GetStringParameter(int parameter)
    {
        if (GetData())
            return m_parameters[parameter];
        else return null;
    }

    /// <summary>
    /// Returns an integer representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>Int</returns>
    public int GetIntParameter(int parameter)
    {
        if (GetData())
            return Int32.Parse(m_parameters[parameter]);
        else return 0;
    }

    /// <summary>
    /// Returns a float representing the specific parameter from the Arduino
    /// </summary>
    /// <param name="parameter">
    /// Specify which parameter to return</param>
    /// <returns>float</returns>
    public float GetFloatParameter(int parameter)
    {
        if (GetData())
            return float.Parse(m_parameters[parameter]);
        else return 0;
    }

    private bool SendString(string send)
    {
        if (m_port.IsOpen)
            try
            {
                m_port.Write(send);
                return true;
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Port is non existent.");
                if (m_found)
                    m_closed = true;
                return false;
            }
        else
        {
            Console.WriteLine("Port is closed.");
            if (m_found)
                m_closed = true;
            return false;
        }
    }

    protected bool GetData()
    {
        if (SendString("SEND"))
        {

            try { m_message = m_port.ReadExisting(); }
            catch (System.IO.IOException e) { m_message = ""; }

            //Split the message into parameters
            m_message.Trim();
            m_parameters.Clear();
            m_parameters.AddRange(m_message.Split('\n'));

            for (int i = 0; i < m_parameters.Count; i++)
            {
                String[] paramData = m_parameters[i].Trim().Split('=');
                if (paramData.Length > 2)
                    throw new Exception("Illegible parameter data received from arduino.");

                if (paramData[0].StartsWith("ANALOG"))
                {
                    paramData[0] = paramData[0].Remove(0, 6);

                    int analogID = int.Parse(paramData[0]);

                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    float analogValue = float.Parse(paramData[1], NumberStyles.Any, ci);

                    m_analogs[analogID] = analogValue;
                    m_parameters[i] = analogValue.ToString();
                }
                else if (paramData[0].StartsWith("DIGITAL"))
                {
                    paramData[0] = paramData[0].Remove(0, 7);
                    int digitalID = int.Parse(paramData[0]);
                    bool digitalValue = int.Parse(paramData[1]) == 1;

                    m_digitals[digitalID] = digitalValue;
                    m_parameters[i] = digitalValue.ToString();
                }
            }
            return true;
        }
        return false;
    }

    public virtual void Destroy()
    {
        m_port.Close();
    }

    //Deconstructors in C# ??? is this even legal
    ~ArduinoInterface()
    {
        m_port.Close();
    }
}