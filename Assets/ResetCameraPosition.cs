using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCameraPosition : MonoBehaviour
{
    public Transform cameraOffset; // This is the offset you apply to the XR camera
    public Vector3 desiredPosition; // The desired position relative to the origin

    void Start()
    {
        ResetPosition();
    }

    void ResetPosition()
    {
        // Ensure the camera is centered relative to the rig's origin
        cameraOffset.localPosition = desiredPosition;
    }
}
