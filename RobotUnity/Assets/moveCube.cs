using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCube : MonoBehaviour
{
    private Rigidbody rb;

    public float force;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButton("w"))
        {
            rb.AddForce(Vector3.forward * force);
        }
    }
}
