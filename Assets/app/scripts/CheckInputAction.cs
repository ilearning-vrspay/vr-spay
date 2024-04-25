using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckInputAction : MonoBehaviour
{
    private InstructionDeliveryController instructionDeliveryController;
    private bool checking = false;
    public int IndexCheck;
    public InputActions InputAction;
    [SerializeField] public UnityEvent CheckEvent = new UnityEvent(); // event that fires when instructions are done
    
    //public void StartChecking()
    //{
    //    checking = true;
    //}


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
        if (InputAction.PoseIndex == IndexCheck){
            CheckEvent.Invoke();
            checking = false;
            this.enabled = false;
        }
    }
}
