using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TriggerToolOutline : MonoBehaviour
{
    private Dictionary<ToolObjectReference, List<Outline>> toolOutlines = new Dictionary<ToolObjectReference, List<Outline>>();
    private ToolObjectReference closestTool = null;
    private bool triggered = false;
    private bool findClosestTool = false;


    private void Update() {
        if (findClosestTool){
            UpdateClosestTool();
        }
        if (closestTool != null){
            foreach (var outline in closestTool.Outlines){
                if (outline.enabled == false && closestTool != null){
                    Debug.Log("doing something");
                    RemoveOutline(closestTool.ColliderObject);

                }
            }
            
        }
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
                if (closestTool.highlighted == true){
                    foreach (var outline in toolOutlines[closestTool]) {
                        outline.OutlineColor = Color.yellow;
                    }
                } else {
                    SetOutlinesEnabled(closestTool, false);
                }
            }
            if (newClosestTool != null) {
                // Enable outlines for the new closest tool
                if (newClosestTool.highlighted == true){

                    foreach (var outline in toolOutlines[newClosestTool]) {
                        outline.OutlineColor = Color.green;
                        
                    }
                
                } else {
                    SetOutlinesEnabled(newClosestTool, true);

                }
                
            }
            closestTool = newClosestTool;
        }
    }

    private void OnTriggerEnter(Collider other) {
        findClosestTool = true;
        Debug.Log("triggered");
        triggered = true;
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

    private void RemoveOutline(Collider other){
        Debug.Log("removing outline");
        triggered = false;
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

    private void OnTriggerExit(Collider other) {
        Debug.Log("trigger exit");
        findClosestTool = false;
       
        GetToolReferenceObject toolRef = other.GetComponent<GetToolReferenceObject>();
        ToolObjectReference toolComponent = null;
        if (toolRef != null)
        {
            toolComponent = toolRef.ToolObjectReference;
        }
        if (toolComponent.highlighted == true){
            foreach (var outline in toolOutlines[closestTool]) {
                outline.OutlineColor = Color.yellow;
            }
            triggered = false;
            toolOutlines.Remove(toolComponent); // Remove the tool from tracking
            if (closestTool == toolComponent) {
                closestTool = null; // Reset closest tool if the exiting tool was the closest
            }
        } else {
            RemoveOutline(other);

        }
        
    }

    // Helper method to enable or disable all outlines for a tool
    private void SetOutlinesEnabled(ToolObjectReference tool, bool enabled) {
        Debug.Log("setting outlines enabled");
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
