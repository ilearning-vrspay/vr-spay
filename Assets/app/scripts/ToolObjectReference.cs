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
    public List<GameObject> RightToolVariations;
    public GameObject LeftToolKitTool;
    public List<GameObject> LeftToolVariations;

    public List<Outline> Outlines;
    public void ToggleToolBeltTool(string hand, bool state)
    {
        if (hand == "Right"){
            Debug.Log("Right tool belt tool toggled");
            RightToolKitTool.SetActive(state);
            //if the right tool has variations
            if (RightToolVariations.Count > 0)
            {
                Debug.Log("Right tool has variations");
                ToggleToolVariation(hand, 0);
            }
        }
        else if (hand == "Left")
        {
            LeftToolKitTool.SetActive(state);
            if (LeftToolVariations.Count > 0)
            {
                ToggleToolVariation(hand, 0);
            }
        }
    }

    public void ToggleToolVariation(string hand, int index)
    {
        if (hand == "Right")
        {
            if (RightToolVariations.Count > 0)
            {
                for (int i = 0; i < RightToolVariations.Count; i++)
                {
                    if (i == index)
                    {
                        RightToolVariations[i].SetActive(true);
                    }
                    else
                    {
                        RightToolVariations[i].SetActive(false);
                    }
                }
            }
        }
        else if (hand == "Left")
        {
            if (LeftToolVariations.Count > 0)
            {
                for (int i = 0; i < LeftToolVariations.Count; i++)
                {
                    if (i == index)
                    {
                        LeftToolVariations[i].SetActive(true);
                    }
                    else
                    {
                        LeftToolVariations[i].SetActive(false);
                    }
                }
            }
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
        Debug.Log("EnableOutline called");
        foreach (var outline in Outlines)
        {
            if (outline != null)
                {
                    outline.enabled = true;
                }
        }
        
    }

    public void DisableOutline()
    {
        Debug.Log("DisableOutline called");
        foreach (var outline in Outlines)
        {
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }
}
