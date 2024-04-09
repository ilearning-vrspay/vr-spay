using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(HandInteractionSystem))]
public class InputActions : MonoBehaviour
{
    private ToolObjectReference isTriggered = null;
    private Animator handAnimator;
    private ToolObjectReference grabbedTool = null;
    private RuntimeAnimatorController baseAnimatorController;
    private AnimatorOverrideController altController;
    private int poseIndex = 0;
    bool JoystickClickable = false;
    private List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
    private readonly string[] animationNames = {
        "DefaultPose",
        "Pose2",
        "Pose3",
        "Pose4",
        "Pose5",
        "Pose6",
        "Pose7",
        "Pose8"
    };

    void Start()
    {
        HandInteractionSystem handInteractionSystem = GetComponent<HandInteractionSystem>();
        handAnimator = handInteractionSystem.handAnimator;
        baseAnimatorController = handAnimator.runtimeAnimatorController;
    }

    
    /// <summary>
    /// Changes the controller used by the hand animator.
    /// </summary>
    /// <param name="reset">If true, resets the controller to the base animator controller.</param>
    /// <param name="controller">The new animator override controller to use.</param>
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

    /// <summary>
    /// Triggers the "Trigger" boolean input action.
    /// </summary>
    /// <param name="value">The boolean value of the input action.</param>
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

    /// <summary>
    /// Chooses a specific pose based on the given index.
    /// </summary>
    /// <param name="index">The index of the pose to choose.</param>
    public void ChooseSpecificPose(int index)
    {

        // handAnimator.SetFloat(animationNames[index], 1.0f);
        // handAnimator.SetFloat(animationNames[poseIndex], 0.0f);
        poseIndex = index;

    }


    /// <summary>
    /// Handles the click event of the joystick.
    /// </summary>
    public void JoystickOnClick()
    {
        if (grabbedTool == null) return;    //  If no tool is currently in hand, return

        int prevPoseIndex = poseIndex; // to act on the previous pose index

        // Loop through animation names with a maximum of the total number of animations to avoid infinite looping
        for (int i = 0; i < animationNames.Length; i++)
        {
            poseIndex = (poseIndex + 1) % animationNames.Length;

            if (IsValidAnimation(poseIndex)) // If the current pose has a valid animation
            {
                handAnimator.SetFloat(animationNames[poseIndex], 1.0f); // turn on the animation for the current pose
                handAnimator.SetFloat(animationNames[prevPoseIndex], 0.0f); // turn off the animation for the previous pose

                // ---- FIX MID ALT STATE TOOL SWITCH BUG HERE ---- //

                break; // Exit the loop after setting the animation
            }
        }
    }



    /// <summary>
    /// Function for the trigger analog input.
    /// </summary>
    /// <param name="value">The value of the analog input.</param>
    //// TRIGGER ANALOG INPUT FUNCTION //
    public void TriggerAnalogInput(float value)
    {
        if (grabbedTool) // If a tool is currently in hand
        {
            if (HasAltState(poseIndex)){ // If the current pose has an alternative state
                handAnimator.SetFloat(animationNames[poseIndex], 1.0f - value);         // Interpolate from current pose
                handAnimator.SetFloat(animationNames[poseIndex] + "_AltState", value);  // to current pose alternative state
            }
        }
    }


    /// <summary>
    /// Handles the input when a tool is pressed.
    /// </summary>
    /// <param name="tool">The reference to the tool object.</param>
    /// <param name="state">The state of the input (pressed or not).</param>
    //// TOOL PICKUP FUNCTION // // could certainly use a better function name
    public void InputPressed(ToolObjectReference tool, bool state, string side = "Right")
    {
        if (!handAnimator.GetBool("PickingUpTool")) // If the hand is not currently picking up a tool
        {
            handAnimator.SetBool("PickingUpTool", state); // Set the picking up tool state
        }
        if (state){                                                 // If the input is pressed to activate object (true)
            handAnimator.SetFloat(animationNames[poseIndex], 1.0f); // Set the current pose animation to 1.0f

        } else {                                                    // If the input is released to deactivate object (false)
            handAnimator.SetFloat(animationNames[poseIndex], 0.0f); // Set the current pose animation to 0.0f
            poseIndex = 0;                                          // Reset the pose index to the default pose
        }
        tool.ToggleToolBeltTool(side, state);                    

    }

    /// <summary>
    /// Called when a collider enters the trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        GetToolReferenceObject toolRef = other.GetComponent<GetToolReferenceObject>(); // First, use the GetToolReferenceObject component to get the tool reference
        if (toolRef != null)    // If the tool reference is not null
        {
            isTriggered = toolRef.ToolObjectReference;                      // Set the tool reference to the triggered tool
            altController = isTriggered.GetOverrideController("Right");     // Set the alternative controller to the Left or Right override controller
        }
    }

    /// <summary>
    /// Called when a collider exits the trigger.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        isTriggered = null;
    }


    /// <summary>
    /// Checks if the specified animation index is a valid animation for the given side.
    /// </summary>
    /// <param name="index">The index of the animation.</param>
    /// <param name="side">The side of the animation (default is "R" for right).</param>
    /// <returns>True if a valid override animation is found, otherwise false.</returns>
    bool IsValidAnimation(int index, string side = "R")
    {
        foreach (var pair in overrides)
        {
            if (pair.Key.name == $"{side}_{animationNames[index]}" && pair.Value != null) // if the current tool has an override for the current animation and the override is a valid animation
            {
                return true; // Valid override found
            }
        }
        return false; // No valid override found
    }

    /// <summary>
    /// Checks if the specified animation index has an alternative state for the given side.
    /// </summary>
    /// <param name="index">The index of the animation.</param>
    /// <param name="side">The side to check for (default is "R").</param>
    /// <returns>True if the animation has an alternative state, false otherwise.</returns>
    bool HasAltState(int index, string side = "R")
    {
        foreach (var pair in overrides)
        {
            if (pair.Key.name == $"{side}_{animationNames[index]}_AltState" && pair.Value != null) //check if current animation has an alternative state
            {
                return true; // Return true if the override has an alternative state
            }
        }
        return false; // No valid override found
    }

}