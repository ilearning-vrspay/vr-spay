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
        // Initialize closest distance to maximum possible value
        float closestDistance = float.MaxValue;
        
        // Variable to store the reference of the new closest tool
        ToolObjectReference newClosestTool = null;
    
        // Iterate through all tools in the dictionary to find the closest one
        foreach (var tool in toolOutlines.Keys) {
            // Calculate squared distance between tool's position and current object's position
            float distance = (tool.transform.position - transform.position).sqrMagnitude;
            
            // If this tool is closer than previously recorded one, update closest distance and tool reference
            if (distance < closestDistance) {
                closestDistance = distance;
                newClosestTool = tool;
            }
        }
    
        // Check if the closest tool has changed
        if (closestTool != newClosestTool) {
            // If there was a previously closest tool, disable its outlines
            if (closestTool != null) {
                if (closestTool.highlighted == true) {
                    // Change outline color to yellow for highlighted previous tool
                    foreach (var outline in toolOutlines[closestTool]) {
                        outline.OutlineColor = Color.yellow;
                    }
                } else {
                    // Otherwise, completely disable the outlines
                    SetOutlinesEnabled(closestTool, false);
                }
            }
    
            // If there is a new closest tool, enable its outlines
            if (newClosestTool != null) {
                if (newClosestTool.highlighted == true) {
                    // Change outline color to green for highlighted new closest tool
                    foreach (var outline in toolOutlines[newClosestTool]) {
                        outline.OutlineColor = Color.green;
                    }
                } else {
                    // Otherwise, enable the outlines normally
                    SetOutlinesEnabled(newClosestTool, true);
                }
            }
    
            // Update the reference of the closest tool
            closestTool = newClosestTool;
        }
    }
    

    private void OnTriggerEnter(Collider other) {
        // Attempt to retrieve the ToolObjectReference component from the collider 'other'
        ToolObjectReference toolComponent = GetToolObjectReference(other);
        if (toolComponent == null || toolComponent.Grabbable == false) {
            return;
        }
        // Indicate that we need to find the closest tool
        findClosestTool = true;
        
        // Log a debug message to indicate that the trigger has been activated
        Debug.Log("triggered");
        
        // Set the triggered flag to true
        triggered = true;
        
        
        
        // Check if the toolComponent is valid and has mesh objects
        if (toolComponent != null && toolComponent.MeshObjects.Count > 0) {
            
            // Create a list to store Outline components associated with this tool's meshes
            List<Outline> outlines = new List<Outline>();
            
            // Iterate over each mesh in the tool's MeshObjects
            foreach (var mesh in toolComponent.MeshObjects) {
                // Try to get the Outline component from the mesh
                Outline outline = mesh.GetComponent<Outline>();
                
                // If an Outline component exists, add it to the outlines list
                if (outline != null) {
                    outlines.Add(outline);
                    // Initially, do not enable the outline; it will be enabled in UpdateClosestTool if this tool is the closest
                }
            }
            
            // Store the outlines associated with this toolComponent in the toolOutlines dictionary
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
