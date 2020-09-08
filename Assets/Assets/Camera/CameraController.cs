using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController camera;

    public Vector3 offSet,startRotation;
    public Transform Target;
    public float minZoom, maxZoom;
    public float desmoothFactor;
    public bool turnWithTarget,smoothRotation,smoothPosition,AutoTakePosition=true,validate;

    float CamZoom;
    Vector3 rotation;

    public void Awake()
    {
        camera = this;
        CamZoom = minZoom;
        rotation = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        if(AutoTakePosition && Target != null)
        {
            TakePosition(Target.position);
            Rotate(Vector3.zero);
        }
    }
    
    public void ControlWithMouse(float sensivity,float zoomSensivity)
    {
        float x =-Input.GetAxis("Mouse Y");
        float y = Input.GetAxis("Mouse X");

        CamZoom = Mathf.Clamp(CamZoom + Input.GetAxis("Mouse ScrollWheel")*zoomSensivity, minZoom, maxZoom);

        Rotate(new Vector2(x, y)*sensivity);
    }
    public void ControlWithTouch(float sensivity,float zoomSensivity)
    {
        foreach(Touch t in Input.touches)
        {
            Rotate(t.deltaPosition*sensivity);
        }

        if (Input.touchCount > 1)
        {
            float deltaX = Input.GetTouch(0).deltaPosition.x - Input.GetTouch(1).deltaPosition.x;
            Zoom(deltaX * zoomSensivity * Time.deltaTime);
        }
    }

    public void SetBaseRotation(Vector3 rotation)
    {
        this.rotation = rotation;
    }
    public void TakePosition(Vector3 targetpos)
    {
        Vector3 CameraPosition = targetpos + offSet - transform.forward * CamZoom;

         //  CameraPosition = Target.TransformPoint(-transform.forward * CamZoom + offSet);


        if (smoothPosition)
            transform.position = Vector3.Lerp(transform.position,CameraPosition, Time.deltaTime * desmoothFactor);
        else
            transform.position = CameraPosition;
    }
    public void Rotate(Vector3 I)
    {
        rotation +=I;

        Vector3 resultV=rotation;
        if (turnWithTarget)
            resultV += Target.rotation.eulerAngles;

        if (smoothRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(resultV), Time.deltaTime*desmoothFactor);
        else
            transform.rotation = Quaternion.Euler(resultV);
    }
    public void Zoom(float deltadistance)
    {
        CamZoom =Mathf.Clamp( deltadistance,minZoom,maxZoom);
    }
    public void SetZoom(float zoom)
    {
        CamZoom = zoom;
    }
    public void ManualUpdate()
    {
        TakePosition(Target.position);
        Rotate(Vector3.zero);
    }
    public void OnValidate()
    {
        if (validate)
        {
            CamZoom = minZoom;
            TakePosition(Target.position);
        }
    }


    [System.Serializable]
    public class Transition
    {
        public Vector3 offset;
        public Vector3 BaseRotation;
        public float deSmoothFactor;
        public Transform carrier;
        public float zoom;

        public void MakeTransition()
        {
            CameraController.camera.smoothPosition = true;
            CameraController.camera.smoothRotation = true;
            CameraController.camera.AutoTakePosition = true;
            CameraController.camera.turnWithTarget = false;

            CameraController.camera.Target = carrier;
            CameraController.camera.offSet = offset;
            CameraController.camera.SetBaseRotation(BaseRotation);
            CameraController.camera.SetZoom(zoom);
            CameraController.camera.desmoothFactor = deSmoothFactor;
        }
    }
}
