using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoComms : MonoBehaviour
{
    [Range(1,90)]
    public int IntMessage;
    public float ResetSpeed = 1;

    public string COMChannel;
    SerialPort ArduinoChannel;

    public bool Toggle;
    public string StringMessage;

    public bool Home = false;
    public bool SendString;

    void Start()
    {
        ArduinoChannel = new SerialPort(COMChannel, 9600);

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
            message = IntMessage.ToString();
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

            }
        }
    }

    void ResetPos()
    {
        float homeFloat = Mathf.Lerp(IntMessage, 90, Time.deltaTime * ResetSpeed);
        IntMessage = Mathf.RoundToInt(homeFloat);
    }

    private void OnDestroy()
    {
        ArduinoChannel.Write("Off");
        ArduinoChannel.Close();
    }
}
