using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

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
    private bool SerialActive;

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
        OpenArduinoChannel();
    }

    void OpenArduinoChannel()
    {
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
            //message = IntMessage.ToString();
            message = $"{Messages[5]},{Messages[4]},{Messages[3]},{Mathf.Abs(Messages[2] - 170)},{Mathf.Abs(Messages[1] - 170)},{Mathf.Abs(Messages[0] - 170)}";
            Debug.Log(message);
        }

        //UnityEngine.Profiling.Profiler.BeginSample("Send Messages");
        SendMessageSerial(message);
        //UnityEngine.Profiling.Profiler.EndSample();

    }

    void SendMessageSerial(string message)
    {
        if (ArduinoChannel.IsOpen && SerialActive)
        {
            try
            {
                ArduinoChannel.WriteLine(message);
            }

            catch (Exception ex)
            {
                Debug.Log("<<< catch : " + ex.ToString());

                SerialActive = false;
                StartCoroutine(OnConnectionFail());                
            }
        }
    }

    IEnumerator OnConnectionFail()
    {
        ArduinoChannel.Close();

        yield return new WaitForSeconds(0.1f);

        OpenArduinoChannel();
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
