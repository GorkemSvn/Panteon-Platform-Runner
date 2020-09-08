using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 rotation;

    Rigidbody rigidbody;
    Vector3 rot;
    public void Start()
    {
        rigidbody=GetComponent<Rigidbody>();
        rigidbody.centerOfMass =Vector3.zero;
        rot = transform.eulerAngles;
    }
    public void FixedUpdate()
    {
        rot += Time.fixedDeltaTime * rotation;
        rigidbody.MoveRotation(Quaternion.Euler(rot));
    }
}
