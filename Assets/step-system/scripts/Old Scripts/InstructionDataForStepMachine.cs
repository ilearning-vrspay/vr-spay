using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InstructionData: MonoBehaviour {

    public static InstructionData Instance;

    void Awake() {
        Instance = this; // data will be referenced by many other scripts
    }
    
    public GameObject[] InstructionSet => instructionSet;
    private GameObject[] instructionSet;

    public int CurrStepIdx {get; set;}
    
    private GameObject currentStep;
    private GameObject stepStateMachine;
    private GameObject instructingState;
   
    // Start is called before the first frame update
    void Start()
    {
        instructionSet = GameObject.FindGameObjectsWithTag("Instruction");
    }

    // logic no longer relevant due to removal of state machines
    // public int FindCurrStepIdx() {
    //     foreach (GameObject instruction in instructionSet) {
    //         if (instructionSet == null) {
    //             Debug.Log("Empty Instruction Set!");
    //             break;
    //         }

    //         GameObject stepStateMachine = instruction.transform.GetChild(0).gameObject; // child of instruction
    //         GameObject instructingState = stepStateMachine.transform.GetChild(1).gameObject; // child of instruction stepmachine
    //         if (instructingState.activeSelf == true) {
    //             CurrStepIdx = Array.IndexOf(instructionSet, instruction);
    //             Debug.Log(CurrStepIdx);
    //             return CurrStepIdx;
    //         }
    //     }
    //     return CurrStepIdx;
    // }

    public GameObject CurrentStep() {
        currentStep = instructionSet[CurrStepIdx];
        return currentStep;
    }
}
