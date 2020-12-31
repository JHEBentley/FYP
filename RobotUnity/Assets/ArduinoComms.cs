using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoComms : MonoBehaviour
{
    [Header("Messages")]
    [Range(1,90)] public int[] Messages;

    [Space(5)][Header("Characteristics")]
    public float ResetSpeed = 1;

    public string COMChannel;
    public int BaudRate = 9600;
    SerialPort ArduinoChannel;

    public bool Toggle;
    public string StringMessage;

    public bool Home = false;
    public bool SendString;

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
            message = $"{Messages[0]},{Messages[1]},{Messages[2]},{Messages[3]},{Messages[4]},{Messages[5]}";
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
            float homeFloat = Mathf.Lerp(Messages[i], 90, Time.deltaTime * ResetSpeed);
            Messages[i] = Mathf.RoundToInt(homeFloat);
        }
    }

    private void OnDestroy()
    {
        ArduinoChannel.Write("Off");
        ArduinoChannel.Close();
    }
}
