using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour
{
    public Transform target;
    public Motor[] Motors;
    public float[] MotorAngles;
    [Space(10)]
    [Range(1,20)]
    public int SamplingDistance;
    public int Speed;
    [Tooltip("0.01 for very accurate, 0.5 for actual application.")]
    public float distanceTolerance;


    void Start()
    {
        float[] angles = new float[Motors.Length];

        for (int i = 0; i < Motors.Length; i++)
        {
            switch (Motors[i].GetAxis())
            {
                case 'x':
                    angles[i] = Motors[i].transform.localRotation.eulerAngles.x;
                    break;
                case 'y':
                    angles[i] = Motors[i].transform.localRotation.eulerAngles.y;
                    break;
                case 'z':
                    angles[i] = Motors[i].transform.localRotation.eulerAngles.z;
                    break;
                default:
                    angles[i] = 0;
                    break;
            }
        }

        MotorAngles = angles;
    }

    
    void Update()
    {
        //JB: If the arm has has gotten close enough to the target, stop calculating IK
        if (DistanceFromTarget(target.position, MotorAngles) > distanceTolerance)
        {
            for (int i = Motors.Length - 1; i >= 0; i--)
            {
                //Calculate inverse kinetics using gradient descent
                float gradient = GradientDescent(target.position, MotorAngles, i);
                MotorAngles[i] -= Speed * gradient;

                Motors[i].Reposition(MotorAngles[i]);
            }
        }
    }

    public float GradientDescent(Vector3 target, float[] angles, int i)
    {
        float tempAngle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);
        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);
        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        angles[i] = tempAngle;
        return gradient;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        return Vector3.Distance(FK_Update(angles), target);
    }

    public Vector3 FK_Update(float[] angles)
    {
        Vector3 lastRot = Motors[0].transform.position;
        Quaternion currentRot = Quaternion.identity;

        for (int i = 1; i < Motors.Length; i++)
        {
            int currentIndex = i - 1;

            currentRot = currentRot * Quaternion.AngleAxis(angles[currentIndex], Motors[currentIndex].quatAxis);
            Vector3 nextRot = lastRot + currentRot * Motors[i].StartPos;

            lastRot = nextRot;
        }
        return lastRot;
    }
}
