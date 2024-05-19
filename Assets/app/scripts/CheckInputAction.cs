using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckInputAction : MonoBehaviour
{
    private InstructionDeliveryController instructionDeliveryController;
    public enum checkType
    {
        Pose,
        ToolVariation,
        GripSqueeze,
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
        Debug.Log("is onnnnnnnn");

        Debug.Log("checking index");
        if (CheckType == checkType.Pose)
        {
            if (InputAction.PoseIndex == IndexCheck){
            CheckEvent.Invoke();
            this.enabled = false;
            }
        }
        else if (CheckType == checkType.ToolVariation)
        {
            if (InputAction.ToolVariationIndex == IndexCheck)
            {
                CheckEvent.Invoke();
                this.enabled = false;
            }
        }
        else if (CheckType == checkType.GripSqueeze)
        {
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

        }
        
    }
}
