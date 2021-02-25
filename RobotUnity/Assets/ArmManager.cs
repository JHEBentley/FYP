using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour
{
    public Transform target;
    public Motor[] motors;
    public float[] motorAxes;
    [Space(10)]
    [Range(1, 20)]
    public int samplingOffset;
    public int speed;
    [Tooltip("0.01 for very accurate, 0.5 for actual application.")]
    public float distanceTolerance;


    void Start()
    {
        float[] axes = new float[motors.Length];

        //Define which axis each motor is restricted to
        for (int i = 0; i < motors.Length; i++)
        {
            switch (motors[i].GetAxis())
            {
                case 'x':
                    axes[i] = motors[i].transform.localRotation.eulerAngles.x;
                    break;
                case 'y':
                    axes[i] = motors[i].transform.localRotation.eulerAngles.y;
                    break;
                case 'z':
                    axes[i] = motors[i].transform.localRotation.eulerAngles.z;
                    break;
                default:
                    axes[i] = 0;
                    break;
            }
        }

        motorAxes = axes;
    }


    void Update()
    {
        Vector3 basePosition = motors[0].transform.position;

        //If the effector has has gotten close enough to the target, stop calculating IK
        if (Vector3.Distance(GetNewEffectorTarget(motorAxes, basePosition, Quaternion.identity), target.position) > distanceTolerance)
        {
            //For each motor
            for (int i = motors.Length - 1; i >= 0; i--)
            {
                //Store the motor angle values so that they can be restored after the theoretical calulations are finished
                //Otherwise the chain will begin to move uncontrollably
                float valueHolder = motorAxes[i];

                //Calculate partial gradient
                //Vector3.Distance is a built in function which calculates the distance between two points using cartesian coordinates
                float d1 = Vector3.Distance(GetNewEffectorTarget(motorAxes, basePosition, Quaternion.identity), target.position);
                motorAxes[i] += samplingOffset;
                float d2 = Vector3.Distance(GetNewEffectorTarget(motorAxes, basePosition, Quaternion.identity), target.position);

                float partialGradient = (d2 - d1) / samplingOffset;

                //Restore original angle values which have not been altered
                motorAxes[i] = valueHolder;

                //Gradient descent calculation
                motorAxes[i] -= speed * partialGradient;

                //Instruct motors to reposition themsevles
                motors[i].Reposition(motorAxes[i]);
            }
        }
    }

    public Vector3 GetNewEffectorTarget(float[] angles, Vector3 targetPos, Quaternion currentRot)
    {
        for (int i = 1; i < motors.Length; i++)
        {
            int lastIndex = i - 1;

            currentRot = currentRot * Quaternion.AngleAxis(angles[lastIndex], motors[lastIndex].quatAxis);
            targetPos = targetPos + currentRot * motors[i].StartPos;
        }

        return targetPos;
    }
}
