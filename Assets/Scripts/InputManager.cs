using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager manager;
    public static Vector2 swipe;
    public static Vector2 normalizedSwipe;
    public static Vector2 deltaPosition;
    public float normalizationRadius;
    public static float sqrdInputPower;
    public static bool swiping { get; private set; }

    Vector2 lastPos;
    public void Awake()
    {
        if(manager==null)
            manager = this;
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            swipe = MouseSwipe();
            normalizedSwipe = Normalize(swipe);
            deltaPosition = (Vector2)Input.mousePosition - lastPos;
            lastPos = Input.mousePosition;
        }
        else
        {
            swipe = TouchSwipe();
            normalizedSwipe = Normalize(swipe);
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                deltaPosition = Input.GetTouch(0).deltaPosition;
        }
        sqrdInputPower = normalizedSwipe.sqrMagnitude;
    }
    Vector2 sp = Vector3.zero;
    Vector2 MouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
            sp = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            swiping = true;
            return ((Vector2)Input.mousePosition - sp);
        }
        swiping = false;

        return Vector2.zero;
    }
    Vector2 TouchSwipe()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                sp = Input.GetTouch(0).position;
            else
            {
                swiping = true;
                return (Input.GetTouch(0).position - sp);
            }
        }
        else
            sp = Vector3.zero;
        swiping = false;

        return Vector2.zero;
    }

    Vector2 Normalize(Vector2 swipe)
    {
        Vector2 nr = swipe / normalizationRadius;

        if (nr.sqrMagnitude > 1)
            nr.Normalize();

        return nr;
    }
}