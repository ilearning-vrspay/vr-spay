using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckInputAction : MonoBehaviour
{
    private InstructionDeliveryController instructionDeliveryController;
    public enum checkType
    {
        PoseChange,
        ToolVariationChange,
        GripSqueeze,
        PickupTool,
        ReleaseTool

    }

    public checkType CheckType;
    public int IndexCheck;
    public float TargetSqueezePercentage = 100.0f;
    public InputActions InputAction;

    private bool hasUsedTool = false;
    [SerializeField] public UnityEvent CheckEvent = new UnityEvent(); // event that fires when instructions are done
    //dropdown list
    
    
    //public void StartChecking()
    //{
    //    checking = true;
    //}


    void Awake()
    {
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        instructionDeliveryController = GetComponent<InstructionDeliveryController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(CheckType)
        {
            case checkType.PoseChange:
                if (InputAction.PoseIndex == IndexCheck)
                {
                    CheckEvent.Invoke();
                    this.enabled = false;
                }
                break;
            case checkType.ToolVariationChange:
                if (InputAction.ToolVariationIndex == IndexCheck)
                {
                    CheckEvent.Invoke();
                    this.enabled = false;
                }
                break;
            case checkType.GripSqueeze:
                float value = InputAction.GripSqueezeValue;
                value = value * 100.0f;

                if (value >= TargetSqueezePercentage)
                {
                    hasUsedTool = true;
                }
                else if (hasUsedTool && value < 0.1f){
                    CheckEvent.Invoke();
                    this.enabled = false;
                    hasUsedTool = false;
                }
                break;
            case checkType.PickupTool:
                if (InputAction.GrabbedTool)
                {
                    CheckEvent.Invoke();
                    this.enabled = false; // should this be hardcoded here? or should it be determined in the inspector?
                }
                break;
            case checkType.ReleaseTool:
                if (!InputAction.GrabbedTool)
                {
                    CheckEvent.Invoke();
                    this.enabled = false;
                }
                break;
        }
        
    }
}
