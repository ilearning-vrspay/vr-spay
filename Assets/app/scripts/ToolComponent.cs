using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventWithTool : UnityEvent<ToolComponent>
{
}

public class ToolComponent : MonoBehaviour
{
    public SO_ToolData ToolData;
    public ToolPoseData toolPoseData;
    public GameObject RightToolkitTool;
    public GameObject LeftToolkitTool;
    public List<GameObject> StageMeshes;

    public int GripIndex;
    //public string GetGripAnimationParamter()
    //{
    //    return "Grip " + GetReadableName();
    //}

    //public string GetUseAnimationParamter()
    //{
    //    return "Use " + GetReadableName();
    //}

    public void IncrementGripIndex()
    {
        if(GripIndex == ToolData.LeftHandGrips.Count - 1)
        {
            GripIndex = 0;
            return;
        }
        GripIndex += 1;
    }

    public string GetReadableName()
    {
        return ToolData.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        StandardHandAndToolAnimationComponent _handThatIsTouching = other.gameObject.GetComponent<StandardHandAndToolAnimationComponent>();
        if(_handThatIsTouching != null)
        {
            _handThatIsTouching.SetTool(this);
        }
    }

    public void ToggleToolBeltTool(string hand, bool state)
    {
        if (hand == "Right"){
            RightToolkitTool.SetActive(state);
        }
        else if (hand == "Left")
        {
            LeftToolkitTool.SetActive(state);
        }

        
    }
}