using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;

public class CameraController : MonoBehaviour
{
    public Rope rope;

    void LateUpdate()
    {
        var cameraPosition = Camera.main.transform.position;
        var centerOfMass = rope.GetCenterOfMass();
        cameraPosition.x = centerOfMass.x;
        cameraPosition.y = centerOfMass.y;
        Camera.main.transform.position = cameraPosition;
    }
}
