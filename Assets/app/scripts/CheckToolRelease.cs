using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckToolRelease : MonoBehaviour
{

    void Awake()
    {
        this.enabled = false;
    }
    public InputActions InputAction;
    [SerializeField] public UnityEvent CheckEvent = new UnityEvent(); // event that fires when instructions are done

    // Update is called once per frame
    void Update()
    {
        Debug.Log("is onnnnnnnn");

        Debug.Log("checking index");
        if (InputAction.GrabbedTool == null)
        {
            Debug.Log("tool is released");
            
            CheckEvent.Invoke();
            this.enabled = false; // should this be hardcoded here? or should it be determined in the inspector?
        }
    }
}
