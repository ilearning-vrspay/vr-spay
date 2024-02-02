using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InstructionNameDisplay : MonoBehaviour
{
    private int currStepIdx;

    private TextMeshProUGUI instructionNameMesh;
    [SerializeField] GameObject Procedure;
    private InstructionDeliveryListController instructionDeliveryListController;

    private void Awake() {
        instructionDeliveryListController = Procedure.GetComponent<InstructionDeliveryListController>();
        instructionNameMesh = GetComponent<TextMeshProUGUI>();
        
    }

    public void DisplayInstruction() { 
        // buttonClickedName = EventSystem.current.currentSelectedGameObject.name; 
        currStepIdx = instructionDeliveryListController.StepIndex;
        instructionNameMesh.text = instructionDeliveryListController.Steps[currStepIdx].gameObject.name;
    }
}
