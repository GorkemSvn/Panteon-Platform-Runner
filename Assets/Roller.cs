using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour
{
    public float angleSpeed;
    public Vector3 moveDir;
    Vector3 startpos;
    void Start()
    {
        startpos = transform.position;
        StartCoroutine(sinus());
    }

    IEnumerator sinus()
    {
        float angle = 0;

        while (gameObject)
        {
            angle += Time.fixedDeltaTime * angleSpeed;

            transform.Rotate(0, 0, -moveDir.x *Time.fixedDeltaTime * Mathf.Cos(angle) * 10);
            transform.position = startpos + moveDir * Mathf.Sin(angle);
            yield return new WaitForFixedUpdate();
        }
    }
}
