using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class StandardHandAndToolAnimationComponent : StandardHandAnimationComponent
{
    public ToolComponent ToolToGrab;

    [SerializeField] private UnityEvent<ToolComponent> onToolGrabbed = new UnityEvent<ToolComponent>();
    [SerializeField] private UnityEvent<ToolComponent> onToolReleased = new UnityEvent<ToolComponent>();
    [SerializeField] private UnityEvent<ToolComponent> onGripSwitched = new UnityEvent<ToolComponent>();
    [SerializeField] private UnityEvent<ToolComponent> onPrimaryPressedWhileToolGrabbed = new UnityEvent<ToolComponent>();

    private bool grabbingTool;

    private bool joystickDown;
    private bool triggerDown;
    private bool primaryButtonDown;
    private bool gripDown;

    private void OnEnable()
    {
        //Debug.Log("HERE");
        //ProcessInputDeviceButton(targetDevice, InputHelpers.Button.PrimaryButton, ref primaryButtonDown,
        //    () => // On Button Down
        //    {
        //        Debug.Log("HERE");
        //        return;
        //    },
        //    () => // On Button Up
        //    {
        //        Debug.Log("HERE 2");
        //        return;
        //    }
        //);
    }

    public bool LockMode;

    // Update is called once per frame
    override public void Update()
    {
        
        // Handle the situation when user connect controller while running the scene
        if (!targetDevice.isValid)
        {
            TryInitialize();
            return;
        }

        //HandleToolGrabAnimationsLock();
        //have a tool that needs to be grabbed

        if(LockMode)
        {
            if (ToolToGrab != null)
            {
                HandleToolGrabAnimationsLock();
                if(grabbingTool)
                {
                    ProcessInputDeviceButton(targetDevice, InputHelpers.Button.Primary2DAxisClick, ref joystickDown,
                        () => // On Button Down
                        {
                            ToggleToolKitTool(false);
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 0.0f);
                            ToolToGrab.IncrementGripIndex();
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 1.0f);
                            ToggleToolKitTool(true);
                            onGripSwitched.Invoke(ToolToGrab);
                            return;
                        },
                        () => // On Button Up
                        {
                            return;
                        }
                    );

                }
                // HandleToolGrabbingAnimations();
            }
            else
            {
                HandleStandardHandAnimations();
            }
        } else
        {
            if (ToolToGrab != null)
            {
                // HandleToolGrabAnimationsLock();
                HandleToolGrabbingAnimations();
            }
            else
            {
                HandleStandardHandAnimations();
            }
        }
        
    }

    public void SetTool(ToolComponent toolToGrab)
    {
        if (!grabbingTool)
            ToolToGrab = toolToGrab;
    }

    public void UnsetTool()
    {
        ToolToGrab = null;
    }

    private bool justReleased;

    private bool isGrabbing;
    private bool isReleasing; 

    private void HandleToolGrabAnimationsLock()
    {
        if (targetDevice.IsPressed(InputHelpers.Button.GripButton, out bool isPressed) && isPressed)
        {
            if (!gripDown) // // this is button down
            {
                //onButtonDown?.Invoke();
                Debug.Log("HERE");
                if(!grabbingTool)
                {
                    if (!handAnimator.GetBool("PickingUpTool"))
                    {
                        handAnimator.SetBool("PickingUpTool", true);
                    }

                    handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 1.0f);
                    ToggleToolKitTool(true);
                    onToolGrabbed.Invoke(ToolToGrab);

                    grabbingTool = true;
                } else
                {
                    if (!handAnimator.GetBool("PickingUpTool"))
                    {
                        handAnimator.SetBool("PickingUpTool", true);
                    }

                    handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 0.0f);
                    ToggleToolKitTool(false);
                    if (handAnimator.GetBool("PickingUpTool"))
                    {
                        handAnimator.SetBool("PickingUpTool", false);
                    }
                    UnsetTool();
                    grabbingTool = false;

                }
            }

            gripDown = true;
            // onButtonHeld?.Invoke();
        }
        else
        {
            if (gripDown) // this is button up
            {
                // onButtonUp?.Invoke();
            }

            gripDown = false;
        }
    }

    private void HandleToolGrabbingAnimations()
    {
        // and if you are pressing the grip button 
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            // grip is starting to be pressed
            if (gripValue >= 0.05f)
            {

                // put controller into tool grab mode
                if (!handAnimator.GetBool("PickingUpTool"))
                {
                    handAnimator.SetBool("PickingUpTool", true);
                }

                handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), gripValue);

                // and the grip is pushed passed threshold (0.8)
                if (gripValue >= 0.8f)
                {
                    if (!grabbingTool)
                    {
                        onToolGrabbed.Invoke(ToolToGrab);
                        grabbingTool = true;
                    }

                    ProcessInputDeviceButton(targetDevice, InputHelpers.Button.Primary2DAxisClick, ref triggerDown,
                        () => // On Button Down
                        {
                            ToggleToolKitTool(false);
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 0.0f);
                            ToolToGrab.IncrementGripIndex();
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 1.0f);
                            ToggleToolKitTool(true);
                            onGripSwitched.Invoke(ToolToGrab);
                            return;
                        },
                        () => // On Button Up
                        {
                            return;
                        }
                    );

                    // turn on the tool so it looks like it was grabbed
                    ToggleToolKitTool(true);

                    if (ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).isUsable)
                    {
                        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
                        if (triggerValue >= 0.05f)
                        {
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetUseAnimationKey(), triggerValue);
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 1 - triggerValue);
                        }
                        else
                        {
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetUseAnimationKey(), 0.0f);
                            handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 1.0f);
                        }
                    }

                    ProcessInputDeviceButton(targetDevice, InputHelpers.Button.PrimaryButton, ref primaryButtonDown,
                        () => // On Button Down
                        {
                            onPrimaryPressedWhileToolGrabbed.Invoke(ToolToGrab);
                            return;
                        },
                        () => // On Button Up
                        {
                            return;
                        }
                    );
                }
            }
            // grip value is less than 0.05
            else
            {
                handAnimator.SetFloat(ToolToGrab.ToolData.GetToolGripAnimationAt(ToolToGrab.GripIndex, GetHandednessOfController()).GetGripAnimationKey(), 0.0f);
                if (grabbingTool)
                {
                    onToolReleased.Invoke(ToolToGrab);
                    grabbingTool = false;
                }
                // Go back to regualr hand animation mode
                if (handAnimator.GetBool("PickingUpTool"))
                {
                    handAnimator.SetBool("PickingUpTool", false);
                    ToggleToolKitTool(false);
                    UnsetTool();
                }
            }
        }
    }

    private void ProcessInputDeviceButton(InputDevice inputDevice, InputHelpers.Button button, ref bool _wasPressedDownPreviousFrame, Action onButtonDown = null, Action onButtonUp = null, Action onButtonHeld = null)
    {
        if (inputDevice.IsPressed(button, out bool isPressed) && isPressed)
        {
            if (!_wasPressedDownPreviousFrame) // // this is button down
            {
                onButtonDown?.Invoke();
            }

            _wasPressedDownPreviousFrame = true;
            onButtonHeld?.Invoke();
        }
        else
        {
            if (_wasPressedDownPreviousFrame) // this is button up
            {
                onButtonUp?.Invoke();
            }

            _wasPressedDownPreviousFrame = false;
        }
    }

    private void ToggleToolKitTool(bool state)
    {
        if (GetHandednessOfController() == "Left")
            ToolToGrab.LeftToolkitTool.SetActive(state);
        else if (GetHandednessOfController() == "Right")
            ToolToGrab.RightToolkitTool.SetActive(state);
    }
}
