using UnityEngine;

public class CursorBillboard : MonoBehaviour
{
    public Camera mainCamera;
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 toCameraVector = mainCamera.transform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(toCameraVector, Vector3.up);
        // Apply the rotation.
        transform.rotation = targetRotation;
    }
}
