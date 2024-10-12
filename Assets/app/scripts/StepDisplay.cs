using UnityEngine;
using TMPro;  // Namespace for TextMeshPro

public class StepDisplay : MonoBehaviour
{
    public int currentStepIndex;  // You can update this value from other scripts
    public TextMeshProUGUI stepText;  // Reference to your TextMeshPro text component
    public InstructionDeliveryListController instructionDeliveryListController;
    public bool useSequenceManager;
    
    public SequenceManager sequenceManager;

    void Start()
    {
        if (instructionDeliveryListController == null)
        {
            useSequenceManager = true;
        }
    }
    void Update()
    {   
        if (useSequenceManager)
        {
            instructionDeliveryListController = sequenceManager.instructionDeliveryListControllers[sequenceManager.CurrentSequenceIndex];
        } 
        int currentStepIndex = instructionDeliveryListController.StepIndex;
        string stepItem = instructionDeliveryListController.Steps[currentStepIndex].name;
        if (stepText != null)
            stepText.text = "Current Step: " + stepItem + "           index: " + currentStepIndex.ToString();
    }
}
