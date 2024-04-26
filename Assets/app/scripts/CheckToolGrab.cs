using UnityEngine;
using UnityEngine.Events;

public class CheckToolGrab : MonoBehaviour
{
    public InputActions InputAction;
    public ToolObjectReference ToolToGrab;
    [SerializeField] public UnityEvent CheckEvent = new UnityEvent(); // event that fires when instructions are done

    void Awake()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputAction.GrabbedTool == ToolToGrab)
        {
            CheckEvent.Invoke();
            this.enabled = false; // should this be hardcoded here? or should it be determined in the inspector?
        }
    }
}
