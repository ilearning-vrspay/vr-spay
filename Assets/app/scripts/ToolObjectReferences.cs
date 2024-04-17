using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolObjectReferences : MonoBehaviour
{
    public List<ToolObjectReference> references = new List<ToolObjectReference>();

    public void DisableAllGrabability()
    {
        foreach (var reference in references)
        {
            reference.DisableGrabability();
        }
    }

    private void Start()
    {
        // DisableAllGrabability();

    }
}
