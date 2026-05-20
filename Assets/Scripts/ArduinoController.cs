using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    public string portName = "/dev/cu.usbmodem101";
    public int baudRate = 115200;

    private SerialPort serialPort;

    public bool arrow1Pressed;
    public bool arrow2Pressed;
    public bool arrow3Pressed;
    public bool arrow4Pressed;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);

        // Short timeout so Unity doesn't freeze
        serialPort.ReadTimeout = 10;

        // CRITICAL Mac Fix
        serialPort.DtrEnable = true;

        try
        {
            serialPort.Open();
            Debug.Log("Arduino Port Opened Successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Could not open Arduino port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            bool receivedNewDataThisFrame = false;
            string latestDataString = "";

            try
            {
                // This loop aggressively reads ALL backed-up data in the buffer
                // until it empties out and hits the 10ms timeout.
                while (true)
                {
                    string data = serialPort.ReadLine();

                    if (!string.IsNullOrEmpty(data))
                    {
                        // CRITICAL String Fix: Strip hidden carriage returns
                        data = data.Trim();

                        // Parse every line so we don't miss rapid taps
                        ParseData(data);

                        latestDataString = data;
                        receivedNewDataThisFrame = true;
                    }
                }
            }
            catch (TimeoutException)
            {
                // Normal. We hit the end of the line and the buffer is perfectly caught up!
            }
            catch (Exception e)
            {
                Debug.LogWarning("Serial Error: " + e.Message);
            }

            // Print the real-time data only once per frame
            if (receivedNewDataThisFrame)
            {
                Debug.Log("Real-Time Arduino Data: " + latestDataString);
            }
        }
    }

    void ParseData(string data)
    {
        string[] values = data.Split(',');

        if (values.Length == 4)
        {
            arrow1Pressed = (values[0] == "1");
            arrow2Pressed = (values[1] == "1");
            arrow3Pressed = (values[2] == "1");
            arrow4Pressed = (values[3] == "1");
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}

