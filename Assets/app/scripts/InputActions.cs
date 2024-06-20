using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor;
using static UnityEditor.Progress;

[RequireComponent(typeof(HandInteractionSystem))]

public class InputActions : MonoBehaviour
{

    public UnityEvent OnToolGrabbed = new UnityEvent();
    public UnityEvent OnToolReleased = new UnityEvent();
    public UnityEvent OnToolUsed = new UnityEvent();
    public UnityEvent OnToolRatched = new UnityEvent();
    public UnityEvent OnToolUnratched = new UnityEvent();

    private ToolObjectReference hoveredTool = null;
    private Animator handAnimator;
    private ToolObjectReference grabbedTool = null;
    private RuntimeAnimatorController baseAnimatorController;
    private AnimatorOverrideController altController;
    private int poseIndex = 0;
    private int customPoseIndex = -1;
    private int toolVariationIndex = 0;
    private float gripSqueezeValue = 0.0f;

    public float GripSqueezeValue
    {
        get => gripSqueezeValue;
        set => gripSqueezeValue = value;
    }
    public int PoseIndex
    {
        get => poseIndex;
        set => poseIndex = value;
    }

    public int ToolVariationIndex
    {
        get => toolVariationIndex;
        set => toolVariationIndex = value;
    }

    public int CustomPoseIndex
    {
        get => customPoseIndex;
        set => customPoseIndex = value;
    }

    public ToolObjectReference GrabbedTool
    {
        get => grabbedTool;
    }
    bool JoystickClickable = false;
    bool ToolVariationSwitchable = true;
    private List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

    public void EnableSwitchablity()
    {
        JoystickClickable = true;
    }

    public void DisableSwitchablity()
    {
        JoystickClickable = false;
    }

    public void EnableToolVariationSwitchability()
    {
        ToolVariationSwitchable = true;
    }

    public void DisableToolVariationSwitchability()
    {
        ToolVariationSwitchable = false;
    }

    private readonly string[] animationNames = {
        "DefaultPose",
        "Pose2",
        "Pose3",
        "Pose4",
        "Pose5",
        "Pose6",
        "Pose7",
        "Pose8",
        "RatchetPose"
    };

    private void Update()
    {
        // Debug.Log(handAnimator.GetFloat(animationNames[poseIndex] + "_AltState"));
        if (grabbedTool != null) { 
            if(grabbedTool.ColliderObject.enabled == false)
            {
                hoveredTool = null;
            }
        }
    }

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
    public void ChangeController(AnimatorOverrideController controller=null)
        {
            if (controller == null)
            {
                controller = altController;
            }
            handAnimator.runtimeAnimatorController = controller;
            altController.GetOverrides(overrides);     
        }

    public void ResetController()
    {
        handAnimator.runtimeAnimatorController = baseAnimatorController;
    }

    /// <summary>
    /// Triggers the "Trigger" boolean input action.
    /// </summary>
    /// <param name="value">The boolean value of the input action.</param>
    public void TriggerOnBooleanInput(bool value)
    {
        if (hoveredTool != null){
            if (grabbedTool == null){
                ChangeController();
                InputPressed(hoveredTool, true);
                grabbedTool = hoveredTool;
                OnToolGrabbed.Invoke();
            } else if (grabbedTool == hoveredTool){
                Debug.Log("HERE");
                ResetController();
                InputPressed(grabbedTool, false);
                grabbedTool = null;
                OnToolReleased.Invoke();
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
        int prevPoseIndex = poseIndex; // to act on the previous pose index
        poseIndex = index;

    
        if (HasAltState(prevPoseIndex))  // if the pose we are switching from has an alternative state
        {
            handAnimator.SetFloat(animationNames[prevPoseIndex] + "_AltState", 0.0f);
        }
        handAnimator.SetFloat(animationNames[prevPoseIndex], 0.0f);              // set the new pose to the value of the previous pose
        handAnimator.SetFloat(animationNames[poseIndex], 1.0f); // turn on the animation for the current pose
    }
    private int testerInt = 0;


    public void JoystickClicked2()
    {
        if (grabbedTool == null) return;


    }
    /// <summary>
    /// Handles the click event of the joystick.
    /// </summary>
    public void JoystickOnClick()
    {
        if (!JoystickClickable) return;
        if (grabbedTool == null) return;    //  If no tool is currently in hand, return
        Debug.Log("testerInt: " + testerInt);
        testerInt++;
        int prevPoseIndex = poseIndex; // to act on the previous pose index

        // Loop through animation names with a maximum of the total number of animations to avoid infinite looping
        for (int i = 0; i < animationNames.Length; i++)
        {
            if(customPoseIndex != -1)
            {
                poseIndex = customPoseIndex;
            } else
            {
                poseIndex = (poseIndex + 1) % animationNames.Length;
            }
            
            if (IsValidAnimation(poseIndex)) // If the current pose has a valid animation
            {
                float poseVal = 1.0f;
                float altVal = 0.0f;
                
                if(HasAltState(prevPoseIndex)) // if the pose we are switching from has an alternative state we need to interpolate between the two states 
                                               // with respect to the current position of the blend tree between the pose and it's alternative state
                {
                    poseVal = handAnimator.GetFloat(animationNames[prevPoseIndex]); // get the value of the pose we are switching from
                    altVal = 1.0f - poseVal;                                        // get it's alternative state value as well
                    
                    handAnimator.SetFloat(animationNames[prevPoseIndex], 0.0f);                 // turn off the previous pose
                    handAnimator.SetFloat(animationNames[prevPoseIndex] + "_AltState", 0.0f);   // turn off the previous pose alternative state

                    if (HasAltState(poseIndex))  // if the pose we are switching to has an alternative state as well
                    {
                        handAnimator.SetFloat(animationNames[poseIndex], poseVal);              // set the new pose to the value of the previous pose
                        handAnimator.SetFloat(animationNames[poseIndex] + "_AltState", altVal); // set the new pose alternative state to the value of the previous pose alternative state

                    } else {    // if the pose we are switching to does not have an alternative state9
                                // we can simply turn on the current pose

                        handAnimator.SetFloat(animationNames[poseIndex], 1.0f); // turn on the animation for the current pose

                    }
                    
                } else {    // if the pose we are switching from does not have an alternative state we can simply turn off the previous pose and turn on the current pose
                            
                    handAnimator.SetFloat(animationNames[poseIndex], 1.0f); // turn on the animation for the current pose
                    handAnimator.SetFloat(animationNames[prevPoseIndex], 0.0f); // turn off the animation for the previous pose
                }

                break; // Exit the loop after setting the animation
            }
        }
    }

    private bool hasClosed;
    private bool ShouldCheckUsability;

    public void ToggleUsabilityCheck(bool value)
    {
        ShouldCheckUsability = value;
    }

    /// <summary>
    /// Function for the trigger analog input.
    /// </summary>
    /// <param name="value">The value of the analog input.</param>
    //// TRIGGER ANALOG INPUT FUNCTION //
    public void TriggerAnalogInput(float value)
    {
        // cacheing the button value
        GripSqueezeValue = value;

        // Do you not have a tool?
        if (grabbedTool == null) return;

        // Is the tool ratchetable and are you ratched?
        Ratchetable ratchetableTool = grabbedTool.GetComponent<Ratchetable>();
        if (ratchetableTool != null)
        {
            if (ratchetableTool.CurrentRatchetLevel == 1)
            {
                return;
            } 
        }

        if (grabbedTool) // If a tool is currently in hand
        {
            if (HasAltState(poseIndex))
            { // If the current pose has an alternative state
                handAnimator.SetFloat(animationNames[poseIndex], 1.0f - value);         // Interpolate from current pose
                handAnimator.SetFloat(animationNames[poseIndex] + "_AltState", value);  // to current pose alternative state

                if (ShouldCheckUsability)
                {
                    // check if the tool is used and if it is, invoke OnToolUsed
                    if (value > 0.9f)
                    {
                        hasClosed = true;
                    }

                    if (hasClosed && value < 0.1f)
                    {
                        hasClosed = false;
                        OnToolUsed.Invoke();
                    }
                }
            }
        }
    }

    public void RatchetTool()
    {
        if (!grabbedTool) return; // you have a grabbed tool

        if (GripSqueezeValue < 0.9f) return; // you have a tool and the grip has been press 0.9 so you can ratchet...
        Ratchetable ratchetableTool = grabbedTool.GetComponent<Ratchetable>();
        
        if(ratchetableTool == null) return; // you have a ratchetable tool

        if(ratchetableTool.CurrentRatchetLevel == ratchetableTool.MaxRatchets)
        {
            // you've reached the maximum ratchets so unratchet
            ratchetableTool.CurrentRatchetLevel = 0;
            Debug.Log($"{ratchetableTool.name} ratchet level: {ratchetableTool.CurrentRatchetLevel} - Tool has been unratcheted. Reset pose.");
            handAnimator.SetFloat(animationNames[animationNames.Length - 1], 0.0f);

            float gv = gripSqueezeValue;
            float inv_gv = 1.0f - gv;

            handAnimator.SetFloat(animationNames[poseIndex], inv_gv);
            handAnimator.SetFloat(animationNames[poseIndex] + "_AltState", gv);
            OnToolUnratched.Invoke();
            return;
        }
        
        // ratchet me
        ratchetableTool.CurrentRatchetLevel++;
        Debug.Log($"{ratchetableTool.name} ratchet level: {ratchetableTool.CurrentRatchetLevel} - Tool has been ratched.");
        // 0: default, 1: defaultAlt, 2: pose2, 3: defaultAlt ...., 19: ratchet1, 20: ratchet2
        // 1, 2 -> 16, 18
        handAnimator.SetFloat(animationNames[poseIndex], 0.0f);
        handAnimator.SetFloat(animationNames[poseIndex] + "_AltState", 0.0f);
        handAnimator.SetFloat(animationNames[animationNames.Length - 1], 1.0f);
        OnToolRatched.Invoke();
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

    public void changeCustomTool(int index = -1)
    {
        if (grabbedTool == null) return;
        if (!ToolVariationSwitchable) return;
        if (index != -1)
        {
            toolVariationIndex = index;
        } else
        {
            toolVariationIndex = (toolVariationIndex + 1) % grabbedTool.RightToolVariations.Count;
            if (grabbedTool.RightToolVariationsTest[toolVariationIndex].VariationOverrideController != null)
            {
                ChangeController(grabbedTool.RightToolVariationsTest[toolVariationIndex].VariationOverrideController);
            } else {
                ChangeController();
            }
        }
        
        grabbedTool.ToggleToolVariation("Right", toolVariationIndex);
    }
    

    public void printTest()
    {
        Debug.Log("Test");
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
            hoveredTool = toolRef.ToolObjectReference;                      // Set the tool reference to the triggered tool
            altController = hoveredTool.GetOverrideController("Right");     // Set the alternative controller to the Left or Right override controller
        }
    }

    /// <summary>
    /// Called when a collider exits the trigger.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        hoveredTool = null;
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
                Debug.Log("pair.key: " + pair.Key.name + "pair.value" + pair.Value.ToString());
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