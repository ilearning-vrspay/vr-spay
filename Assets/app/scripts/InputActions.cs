using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputActions : MonoBehaviour
{

    private ToolComponent isTriggered = null;
    public Animator handAnimator;
    public ToolComponent grabbedTool = null;
    public RuntimeAnimatorController baseAnimatorController;
    public ToolComponent TempTool;
    private AnimatorOverrideController animatorOverrideController;
    public AnimatorOverrideController altController;

    public void ChangeController(RuntimeAnimatorController altController)
    {

        // handAnimator.runtimeAnimatorController = animatorOverrideController;
        
    }

    // TRIGGER BOOLEAN INPUT FUNCTION //
    public void TriggerOnBooleanInput(bool value)
    {
        // if (isTriggered != null)
        // {
        ToolPoseData toolPoses = TempTool.toolPoseData;
        Debug.Log("Pose object: " + toolPoses);
        AnimationClip firstPoseClip = toolPoses.defaultAltPose;
        Debug.Log("First Pose: " + firstPoseClip);
        // animatorOverrideController = new AnimatorOverrideController(handAnimator.runtimeAnimatorController);
        handAnimator.runtimeAnimatorController = altController;
        altController["R_OlsenHegar_NeedleDrivers_Pose1_Open"] = firstPoseClip;
        // ChangeController(animatorOverrideController);
        // altController["DefaultPose"] = firstPoseClip;
        // Debug.Log("animatorOverride: " + animatorOverrideController);
        // handAnimator.runtimeAnimatorController = altController;
        // animatorOverrideController.runtimeAnimatorController = baseAnimatorController;
        // handAnimator.SetFloat("DefaultPose", 1.0f);

            // animatorOverrideController["DefaultPose"] = firstPoseClip;

            // handAnimator.runtimeAnimatorController = animatorOverrideController;
        // }
        // if (isTriggered != null){
        //     Debug.Log("DID THE THING MA!   ->   " + value);

        //     if (grabbedTool == null){
        //         InputPressed(isTriggered, true);
        //         grabbedTool = isTriggered;
        //     } else if (grabbedTool == isTriggered){
        //         InputPressed(grabbedTool, false);
        //         grabbedTool = null;
        //     }
        //     // else {do other input for button}

        // } 

    }


    // TRIGGER ANALOG INPUT FUNCTION //
    public void TriggerAnalogInput(float value)
    {
        
        if (grabbedTool)
        {
            Debug.Log("I'm in the place I should be with this many values: " + value);
            handAnimator.SetFloat(grabbedTool.ToolData.GetToolGripAnimationAt(0, "Right").GetUseAnimationKey(), value);
            handAnimator.SetFloat(grabbedTool.ToolData.GetToolGripAnimationAt(0, "Right").GetGripAnimationKey(), 1f - value);

        }
        
    }

    



    public void InputPressed(ToolComponent tool, bool state)
        {

            if (!handAnimator.GetBool("PickingUpTool"))
            {
                handAnimator.SetBool("PickingUpTool", state);
            }
            if (state){
                handAnimator.SetFloat(tool.ToolData.GetToolGripAnimationAt(tool.GripIndex, "Right").GetGripAnimationKey(), 1.0f);
            } else {
                handAnimator.SetFloat(tool.ToolData.GetToolGripAnimationAt(tool.GripIndex, "Right").GetGripAnimationKey(), 0.0f);

            }
            tool.ToggleToolBeltTool("Right", state);


        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("EnteredDisThing");
            isTriggered = other.GetComponent<ToolComponent>();
            Debug.Log(isTriggered);
        }

        private void OnTriggerExit(Collider other)
        {
            isTriggered = null;
        }
}