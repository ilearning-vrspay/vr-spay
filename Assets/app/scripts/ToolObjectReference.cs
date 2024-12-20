using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]



public class ToolVariation
{
    public GameObject ToolVariationObject;
    public AnimatorOverrideController VariationOverrideController;

}


public class ToolObjectReference : MonoBehaviour
{
    private bool grabbable = false;
    public bool Grabbable
    {
        get { return grabbable; }
        set { grabbable = value; }
    }

    public Collider ColliderObject;
    public GameObject RootBone;
    public List<GameObject> MeshObjects;
    public bool highlighted = false;
    public List<Outline> Outlines;
    public AnimatorOverrideController RightOverrideController;
    public AnimatorOverrideController LeftOverrideController;
    public GameObject RightToolKitTool;
    //public List<GameObject> RightToolVariations;
    public List<ToolVariation> RightToolVariations = new List<ToolVariation>();
    public GameObject LeftToolKitTool;
    public List<GameObject> LeftToolVariations;

    

    public void changeOutlineColor(string color)
    {
        foreach (var outline in Outlines)
        {
            if (outline != null)
            {
                if (color == "green")
                {
                    outline.OutlineColor = Color.green;
                }
                else if (color == "yellow")
                {
                    outline.OutlineColor = Color.yellow;
                }
            }
        }
    }
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
                        RightToolVariations[i].ToolVariationObject.SetActive(true);
                    }
                    else
                    {
                        RightToolVariations[i].ToolVariationObject.SetActive(false);
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
        grabbable = true;
        // if (ColliderObject != null)
        // {

        //     ColliderObject.enabled = true;
        // }
    }

    public void DisableGrabability()
    {
        grabbable = false;
        // if (ColliderObject != null)
        // {
        //     ColliderObject.enabled = false;
        // }
    }

    public void EnableOutline()
    {

        foreach (var outline in Outlines)
        {
            if (outline != null)
                {
                    outline.enabled = true;
                }
        }
        
    }

    public void highlightTool()
    {
        highlighted = true;
        EnableOutline();
    }

    public void unhighlightTool()
    {
        highlighted = false;
        DisableOutline();
    }

   


    public void DisableOutline()
    {
        Debug.Log("DisableOutline called");
        foreach (var outline in Outlines)
        {
            if (outline != null)
            {
                outline.enabled = false;
                outline.OutlineColor = Color.yellow;
            }
        }
    }
}
