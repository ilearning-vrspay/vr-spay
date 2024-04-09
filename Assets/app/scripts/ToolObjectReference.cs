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


}
