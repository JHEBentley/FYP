using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRArmManager : MonoBehaviour
{
    //Reference the right controller
    private InputDevice rightController;
    public Transform rightControllerTransform;

    //Get the IDs for each of the required motors
    public int WristID;
    public int neckID;
    public int jawID;

    private ArduinoComms commsScript;

    //Determines the oritentation of the controller in case of any sudden movements
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
            //right controller should be the first (and only) device of this type present
            rightController = presentDevices[0];
        }
    }

    void Update()
    {
        //If we don't have a controller to track, find one and return for this frame
        if (rightController.name == null)
        {
            FindController();
            return;
        }

        //The neck and the wrist are controlled by the rotation of the controller in the up axis and right axis respectively
        commsScript.Messages[neckID] = Mathf.RoundToInt(GetControllerUpRotation(rightControllerTransform));
        commsScript.Messages[WristID] = Mathf.RoundToInt(GetControllerRightRotation(rightControllerTransform));

        //The jaws are controlled by hpw fair in the trigger has been pulled (bwhich is a float clamped between 0 and 1
        float triggerVal = GetTriggerValue(rightController);

        //Set jaw position based on trigger value
        //Clamp to 160 because at 170 sometimes the cogs become detatched.
        float jawPos = Mathf.Clamp(Mathf.Abs((1 - triggerVal) * 180), 10, 160);
        commsScript.Messages[jawID] = Mathf.RoundToInt(jawPos);
    }

    private float GetTriggerValue(InputDevice controller)
    {
        //Get the state of the trigger, should be between 0 and 1
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
