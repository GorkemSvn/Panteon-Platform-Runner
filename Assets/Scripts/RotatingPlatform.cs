using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float turnSpeed;
    public Vector3 axis;
    Rigidbody rigidbody;

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        axis.z += turnSpeed * Time.fixedDeltaTime;
        rigidbody.MoveRotation ( Quaternion.Euler(axis));
    }
}
