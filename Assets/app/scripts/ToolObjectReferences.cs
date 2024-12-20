using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolObjectReferences : MonoBehaviour
{
    public List<ToolObjectReference> references = new List<ToolObjectReference>();
    public bool grabbable = false;

    public void DisableAllGrabability()
    {
        Debug.Log("DisableAllGrabability called");
        foreach (var reference in references)
        {
            reference.DisableGrabability();
        }
    }

    public void EnableAllGrabability()
    {
        Debug.Log("DisableAllGrabability called");
        foreach (var reference in references)
        {
            reference.EnableGrabability();
        }
    }

    public void DisableAllOutlines()
    {
        Debug.Log("DisableAllOutlines called");
        foreach (var reference in references)
        {
            reference.DisableOutline();
        }
    }

    private void Start()
    {
        if (grabbable)
        {
            EnableAllGrabability();
        }
        else
        {
            DisableAllGrabability();
        }
        // DisableAllGrabability();
        changeHighlightColor("green");
    }

    public void changeHighlightColor(string color)
    {
        foreach (var reference in references)
        {
            if (color != null)
            {
                reference.changeOutlineColor(color);
            }
            else
            {
            reference.changeOutlineColor("yellow");
            }
        }
    }
}
