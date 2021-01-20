using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoComms : MonoBehaviour
{
    [Header("Messages")]
    [Range(10, 170)] public int[] Messages;
    [Range(10, 170)] public int[] HomeValues;

    [Space(5)][Header("Characteristics")]
    public float ResetSpeed = 1;

    public string COMChannel;
    public int BaudRate = 9600;
    SerialPort ArduinoChannel;

    public bool Toggle;
    public string StringMessage;

    public bool Home = false;
    public bool SendString;

    public static ArduinoComms instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ArduinoChannel = new SerialPort(COMChannel, BaudRate);

        ArduinoChannel.Open();
        ArduinoChannel.ReadTimeout = 1;
    }

    void Update()
    {
        if (Toggle)
        {
            StringMessage = "Forced message";
        }

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
            //message = IntMessage.ToString();
            message = $"{Messages[5]},{Messages[4]},{Messages[3]},{Mathf.Abs(Messages[2] - 170)},{Mathf.Abs(Messages[1] - 170)},{Mathf.Abs(Messages[0] - 170)}";
            Debug.LogWarning(message);
        }

        if (ArduinoChannel.IsOpen)
        {
            try
            {
                ArduinoChannel.WriteLine(message);
                Debug.Log(ArduinoChannel.ReadLine());
            }

            catch(System.Exception)
            {
                //Debug.LogWarning("Message could not be written to Arduino!");
            }
        }
    }

    void ResetPos()
    {
        for (int i = 0; i < Messages.Length; i++)
        {
            float homeFloat = Mathf.Lerp(Messages[i], HomeValues[i], Time.deltaTime * ResetSpeed);
            Messages[i] = Mathf.RoundToInt(homeFloat);
        }
    }

    private void OnDestroy()
    {
        ArduinoChannel.Write("Off");
        ArduinoChannel.Close();
    }
}
