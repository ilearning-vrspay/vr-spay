using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StandardHandAnimationComponent : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;

    protected InputDevice targetDevice;

    public Animator handAnimator;

    virtual public void Start()
    {
        handAnimator.applyRootMotion = true;
        TryInitialize();
    }

    virtual public void Update()
    {
        // Handle the situation when user connect controller while running the scene
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }

        HandleStandardHandAnimations();
    }

    public string GetHandednessOfController()
    {
        char[] delimiterChars = { ',' };
        if (controllerCharacteristics.ToString().Split(delimiterChars)[1] == " Left")
            return "Left";
        else if (controllerCharacteristics.ToString().Split(delimiterChars)[1] == " Right")
            return "Right";
        else
            return "";
    }

    protected void HandleStandardHandAnimations()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    protected void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }
}
