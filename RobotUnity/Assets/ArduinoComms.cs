using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoComms : MonoBehaviour
{
    [Header("Messages")]
    //Position instructions to be sent to each motor
    [Range(10, 170)] public int[] Messages;
    //Values to be sent to each motor to trigger an automatic reset to the "home" position (standing upright)
    [Range(10, 170)] public int[] HomeValues;

    [Space(5)][Header("Characteristics")]
    //How quickly the arm reverts to the home position when triggered
    public float ResetSpeed = 1;

    //Serial setup
    public string COMChannel;
    public int BaudRate = 9600;
    SerialPort ArduinoChannel;
    private bool SerialActive;

    //Test message to be sent in the form of a string
    public string StringMessage;

    //Toggling test string on and off, also toggling home positioning on and off
    public bool Home = false;
    public bool SendString;

    //Create a static instance of this class
    public static ArduinoComms instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ArduinoChannel = new SerialPort(COMChannel, BaudRate);
        OpenArduinoChannel();
    }

    void OpenArduinoChannel()
    {
        //Open a new serial channel to communicate with the arduino
        ArduinoChannel.Open();
        ArduinoChannel.ReadTimeout = 1;
        ArduinoChannel.WriteTimeout = 500;

        SerialActive = true;
    }

    void Update()
    {
        if (Home)
        {
            ResetPos();
        }

        string message;

        if (SendString)
        {
            message = StringMessage;
        }

        else
        {
            //Send the string of instructions to each motor, along with their given IDs
            message = $"{Messages[5]},{Messages[4]},{Messages[3]},{Mathf.Abs(Messages[2] - 170)},{Mathf.Abs(Messages[1] - 170)},{Mathf.Abs(Messages[0] - 170)}";

            //Log the message so it can be validated if needs be
            Debug.Log(message);
        }

        SendMessageSerial(message);

    }

    void SendMessageSerial(string message)
    {
        if (ArduinoChannel.IsOpen && SerialActive)
        {
            //Try to send the message to the Arduino
            try
            {
                ArduinoChannel.WriteLine(message);
            }

            //If the arduino is not responding, start the coroutine for the event of a channel failure
            catch (Exception ex)
            {
                //Log the exception for reference
                Debug.LogError("<<< catch : " + ex.ToString());

                SerialActive = false;
                StartCoroutine(OnConnectionFail());                
            }
        }
    }

    IEnumerator OnConnectionFail()
    {
        //Close the channel, wait for 100ms, and attempt to reopen. This prevents the system from getting stuck in an infinite loop in the event of a broken connection
        ArduinoChannel.Close();

        yield return new WaitForSeconds(0.1f);

        OpenArduinoChannel();
    }

    void ResetPos()
    {
        //Tell each motor to go to the home position
        for (int i = 0; i < Messages.Length; i++)
        {
            float homeFloat = Mathf.Lerp(Messages[i], HomeValues[i], Time.deltaTime * ResetSpeed);
            Messages[i] = Mathf.RoundToInt(homeFloat);
        }
    }

    private void OnDestroy()
    {
        //When the scene has been closed, close off the serial channel
        ArduinoChannel.Write("Off");
        ArduinoChannel.Close();
    }
}
