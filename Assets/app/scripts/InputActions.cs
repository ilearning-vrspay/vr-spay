using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;
using Unity.VisualScripting;

public class InputActions : MonoBehaviour
{

    private ToolComponent isTriggered = null;
    public Animator handAnimator;
    public ToolComponent grabbedTool = null;
    public RuntimeAnimatorController baseAnimatorController;
    public ToolComponent TempTool;
    private AnimatorOverrideController animatorOverrideController;
    public AnimatorOverrideController altController;
    private ToolObjectReferences toolObjectReference;
    private int poseIndex = 0;

    /// 
    private List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();



    private readonly string[] animationNames = {
        "DefaultPose",
        "DefaultPose_AltState",
        "Hand_Fist_Pose",
        "Hand_Idle_Pose",
        "Hand_Pinch_Pose",
        "Hand_Point_Pose",
        "Pose2",
        "Pose2_AltState",
        "Pose3",
        "Pose4",
        "Pose5",
        "Pose6",
        "Pose7",
        "Pose8"
    };


    bool IsValidAnimation(int index, string side = "R")
    {
        foreach (var pair in overrides)
        {
            if (pair.Key.name == $"{side}_{animationNames[index]}" && pair.Value != null)
            {
                return true; // Valid override found
            }
        }
        return false; // No valid override found
    }
    public void ChangeController(bool reset=false, AnimatorOverrideController controller=null)
    {

        if (reset)
        {
            handAnimator.runtimeAnimatorController = baseAnimatorController;
        }
        else
        {
            handAnimator.runtimeAnimatorController = altController;
            altController.GetOverrides(overrides);
            
        }
        
    }

    // TRIGGER BOOLEAN INPUT FUNCTION //
    public void TriggerOnBooleanInput(bool value)
    {

        if (isTriggered != null){
            if (grabbedTool == null){
                ChangeController(false, altController);
                InputPressed(isTriggered, true);
                grabbedTool = isTriggered;
            } else if (grabbedTool == isTriggered){
                ChangeController(true);
                InputPressed(grabbedTool, false);
                grabbedTool = null;
            }

        } 

    }

    public void ChooseSpecificPose(int index)
    {

        // handAnimator.SetFloat(animationNames[index], 1.0f);
        // handAnimator.SetFloat(animationNames[poseIndex], 0.0f);
        poseIndex = index;

    }

    bool JoystickClickable = false;

    public void JoystickOnClick()
    {
        if (grabbedTool == null) return;
        Debug.Log("Joystick Clicked");

        int prevPoseIndex = poseIndex; // to act on the previous pose index
        int attempts = 0; // Counter to limit the number of attempts to the number of animations

        // Loop through animation names with a maximum of the total number of animations to avoid infinite looping
        for (int i = 0; i < animationNames.Length; i++)
        {
            poseIndex = (poseIndex + 1) % animationNames.Length;
            if (poseIndex == 1) // Skip the AltState
            {
                poseIndex = 6; // Move to the next iteration immediately
            }

            if (IsValidAnimation(poseIndex))
            {
                handAnimator.SetFloat(animationNames[poseIndex], 1.0f);
                handAnimator.SetFloat(animationNames[prevPoseIndex], 0.0f);
                break; // Exit the loop after setting the animation
            }

            attempts++;
            if (attempts >= animationNames.Length)
            {
                // We've checked all possible animations and found no valid override, break to avoid infinite looping
                Debug.Log("No valid animation found after cycling through all options.");
                break;
            }
        }

        
    }


    // TRIGGER ANALOG INPUT FUNCTION //
    public void TriggerAnalogInput(float value)
    {
        
        if (grabbedTool) // If a tool is currently in hand
        {
            handAnimator.SetFloat("DefaultPose", 1f - value);
            handAnimator.SetFloat($"DefaultPose_AltState", value);
            // handAnimator.SetFloat(grabbedTool.ToolData.GetToolGripAnimationAt(0, "Right").GetUseAnimationKey(), value);
            // handAnimator.SetFloat(grabbedTool.ToolData.GetToolGripAnimationAt(0, "Right").GetGripAnimationKey(), 1f - value);

        }
        
    }

    



    public void InputPressed(ToolComponent tool, bool state)
    {

        if (!handAnimator.GetBool("PickingUpTool"))
        {
            handAnimator.SetBool("PickingUpTool", state);
        }
        if (state){
            handAnimator.SetFloat("DefaultPose", 1.0f);
            // handAnimator.SetFloat(tool.ToolData.GetToolGripAnimationAt(tool.GripIndex, "Right").GetGripAnimationKey(), 1.0f);
        } else {
            handAnimator.SetFloat("DefaultPose", 0.0f);
            // handAnimator.SetFloat(tool.ToolData.GetToolGripAnimationAt(tool.GripIndex, "Right").GetGripAnimationKey(), 0.0f);

        }
        tool.ToggleToolBeltTool("Right", state);


    }

    private void OnTriggerEnter(Collider other)
    {
        GetToolReferenceObject toolRef = other.GetComponent<GetToolReferenceObject>();
        if (toolRef != null)
        {

            toolObjectReference = toolRef.toolReferenceObject;

            altController = toolObjectReference.RightOverrideController;
        }


        isTriggered = other.GetComponent<ToolComponent>();

    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = null;
    }

    // Method to log all properties of an object
    public static void LogAllProperties(object obj)
    {
        Debug.Log("Logging properties of object " + obj);
        if (obj == null)
        {
            Debug.Log("Object is null");
            return;
        }

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            try
            {
                object value = property.GetValue(obj, null);
                Debug.Log(property.Name + ": " + (value?.ToString() ?? "null"));
            }
            catch (TargetInvocationException ex) when (ex.InnerException is NotSupportedException)
            {
                // This catches the NotSupportedException for deprecated properties like 'rigidbody'
                Debug.Log($"Property {property.Name} is deprecated and cannot be accessed.");
            }
            catch (Exception ex)
            {
                // Log other types of exceptions here if necessary
                Debug.Log($"Error accessing property {property.Name}: {ex.Message}");
            }
        }
    }

}