using UnityEngine;
using System.Collections.Generic;

public class TriggerToolOutline : MonoBehaviour
{
    private Dictionary<ToolObjectReference, List<Outline>> toolOutlines = new Dictionary<ToolObjectReference, List<Outline>>();
    private ToolObjectReference closestTool = null;

    private void Update() {
        UpdateClosestTool();
    }

    private void UpdateClosestTool() {
        float closestDistance = float.MaxValue;
        ToolObjectReference newClosestTool = null;

        // Iterate through all tools to find the closest one
        foreach (var tool in toolOutlines.Keys) {
            float distance = (tool.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance) {
                closestDistance = distance;
                newClosestTool = tool;
            }
        }

        // If the closest tool has changed, update outlines
        if (closestTool != newClosestTool) {
            if (closestTool != null) {
                // Disable outlines for the previously closest tool
                SetOutlinesEnabled(closestTool, false);
            }
            if (newClosestTool != null) {
                // Enable outlines for the new closest tool
                SetOutlinesEnabled(newClosestTool, true);
            }
            closestTool = newClosestTool;
        }
    }

    private void OnTriggerEnter(Collider other) {

        ToolObjectReference toolComponent = GetToolObjectReference(other);

        // ToolObjectReference toolComponent = other.GetComponent<ToolObjectReference>();
        if (toolComponent != null && toolComponent.MeshObjects.Count > 0) {
            List<Outline> outlines = new List<Outline>();
            foreach (var mesh in toolComponent.MeshObjects) {
                Outline outline = mesh.GetComponent<Outline>();
                if (outline != null) {
                    outlines.Add(outline);
                    // Initially, do not enable the outline; it will be enabled in UpdateClosestTool if this tool is the closest
                }
            }
            toolOutlines[toolComponent] = outlines;
        }
    }

    private void OnTriggerExit(Collider other) {
        GetToolReferenceObject toolRef = other.GetComponent<GetToolReferenceObject>();
        ToolObjectReference toolComponent = null;
        if (toolRef != null)
        {
            toolComponent = toolRef.ToolObjectReference;
        }
        if (toolComponent != null && toolOutlines.ContainsKey(toolComponent)) {
            SetOutlinesEnabled(toolComponent, false); // Disable outlines when the tool exits
            toolOutlines.Remove(toolComponent); // Remove the tool from tracking
            if (closestTool == toolComponent) {
                closestTool = null; // Reset closest tool if the exiting tool was the closest
            }
        }
    }

    // Helper method to enable or disable all outlines for a tool
    private void SetOutlinesEnabled(ToolObjectReference tool, bool enabled) {
        foreach (var outline in toolOutlines[tool]) {
            outline.enabled = enabled;
        }
    }

    ToolObjectReference GetToolObjectReference(Collider other){
        GetToolReferenceObject toolRef = other.GetComponent<GetToolReferenceObject>();
        ToolObjectReference toolComponent = null;
        if (toolRef != null)
        {
            toolComponent = toolRef.ToolObjectReference;
        }

        return toolComponent;
    }
}
