using UnityEngine;
using TMPro;  // Namespace for TextMeshPro

public class StepDisplay : MonoBehaviour
{
    public int currentStepIndex;  // You can update this value from other scripts
    public TextMeshProUGUI stepText;  // Reference to your TextMeshPro text component
    public InstructionDeliveryListController instructionDeliveryListController;

    void Update()
    {
        int currentStepIndex = instructionDeliveryListController.StepIndex;
        string stepItem = instructionDeliveryListController.Steps[currentStepIndex].name;
        if (stepText != null)
            stepText.text = "Current Step: " + stepItem + "           index: " + currentStepIndex.ToString();
    }
}
