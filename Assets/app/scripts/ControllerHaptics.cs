using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerHaptics : MonoBehaviour
{
    public XRBaseController controller;  // Reference to the XR controller
    public float amplitude;
    public float duration;

    void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<XRBaseController>();
        }
    }

    public void TriggerHapticFeedback()
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }
    }
}
