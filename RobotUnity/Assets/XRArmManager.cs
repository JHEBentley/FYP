using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRArmManager : MonoBehaviour
{
    private InputDevice rightController;
    public Transform rightControllerTransform;

    public int WristID;
    public int neckID;
    public int jawID;

    private ArduinoComms commsScript;

    private bool UpStateLastUp = true;
    [SerializeField]
    private int wristOffset;

    void Start()
    {
        commsScript = ArduinoComms.instance;
    }

    private void FindController()
    {
        //Make a list containing every XR device present
        List<InputDevice> presentDevices = new List<InputDevice>();
        //Define the right controller as a device with both "right" and "controller" characteristics
        InputDeviceCharacteristics rightControllerChars = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //Assign the rightController variable as the device from the present devices list which has these characteristics
        InputDevices.GetDevicesWithCharacteristics(rightControllerChars, presentDevices);

        if (presentDevices.Count > 0)
        {
            rightController = presentDevices[0];
        }
    }

    void Update()
    {
        if (rightController.name == null)
        {
            FindController();
            return;
        }

        commsScript.Messages[neckID] = Mathf.RoundToInt(GetControllerUpRotation(rightControllerTransform));
        commsScript.Messages[WristID] = Mathf.RoundToInt(GetControllerRightRotation(rightControllerTransform));
        //Debug.Log(rightControllerTransform.rotation);

        float triggerVal = GetTriggerValue(rightController);

        //Set jaw position based on trigger value
        //Clamp to 160 because at 170 sometimes the cogs become detatched.
        float jawPos = Mathf.Clamp(Mathf.Abs((1 - triggerVal) * 180), 10, 160);
        commsScript.Messages[jawID] = Mathf.RoundToInt(jawPos);
    }

    private float GetTriggerValue(InputDevice controller)
    {
        controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerState);

        return triggerState;
    }

    private float GetControllerUpRotation(Transform controller)
    {
        Vector3 controllerUpState = controller.up;

        //if x and z are positive, stop
        if (controllerUpState.x > 0 && controllerUpState.z > 0)
        {
            float defaultRot = UpStateLastUp ? 10 : 170;
            return defaultRot;
        }

        // up = 1, rot = 10
        // up = 0, rot = 90
        // up = -1, rot = 170
        float upVal = controllerUpState.y + 1;

        //y = mx + c equation can be simplified using the Lerp function.
        //float rot = (-80 * upVal) + 170;
        float rot = Mathf.Lerp(170, 10, upVal / 2);

        UpStateLastUp = rot > 90 ? false : true;

        return rot;
    }

    private float GetControllerRightRotation(Transform controller)
    {
        Vector3 controllerRightState = controller.right;

        // up = 1, rot = 10
        // up = 0, rot = 90
        // up = -1, rot = 170
        float upVal = controllerRightState.x + 1;

        //y = mx + c equation can be simplified using the Lerp function.
        //float rot = (-80 * upVal) + 170;
        float rot = Mathf.Lerp(10, 170, upVal / 2) + wristOffset;

        //UpStateLastUp = rot > 90 ? false : true;

        return rot;
    }
}
