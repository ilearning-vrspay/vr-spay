using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolObjectReference : MonoBehaviour
{

    public Collider ColliderObject;
    public GameObject RootBone;
    public List<GameObject> MeshObjects;
    public AnimatorOverrideController RightOverrideController;
    public AnimatorOverrideController LeftOverrideController;
    public GameObject RightToolKitTool;
    public GameObject LeftToolKitTool;

    public Outline Outline;
    
    public void ToggleToolBeltTool(string hand, bool state)
    {
        if (hand == "Right"){
            RightToolKitTool.SetActive(state);
        }
        else if (hand == "Left")
        {
            LeftToolKitTool.SetActive(state);
        }
    }

    //function to get left or right override controller
    public AnimatorOverrideController GetOverrideController(string hand)
    {
        if (hand == "Right")
        {
            return RightOverrideController;
        }
        else if (hand == "Left")
        {
            return LeftOverrideController;
        }
        return null;
    }

    public void EnableGrabability()
    {
        if (ColliderObject != null)
        {
            Debug.Log("EnableGrabability called");
            ColliderObject.enabled = true;
        }
    }

    public void DisableGrabability()
    {
        if (ColliderObject != null)
        {
            ColliderObject.enabled = false;
        }
    }

    public void EnableOutline()
    {
        if (Outline != null)
        {
            Outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (Outline != null)
        {
            Outline.enabled = false;
        }
    }
}
