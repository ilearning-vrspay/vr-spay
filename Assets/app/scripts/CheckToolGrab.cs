using UnityEngine;
using UnityEngine.Events;

public class CheckToolGrab : MonoBehaviour
{
    public enum grabType
    {
        Pickup,
        Release,
    }

    public grabType GrabType;
    
    public InputActions InputAction;
    [SerializeField] public UnityEvent CheckEvent = new UnityEvent(); // event that fires when instructions are done

    void Awake()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        switch(GrabType)
        {
            case grabType.Pickup:
                if (InputAction.GrabbedTool)
                {
                    CheckEvent.Invoke();
                    this.enabled = false; // should this be hardcoded here? or should it be determined in the inspector?
                }
                break;
            case grabType.Release:
                if (!InputAction.GrabbedTool)
                {
                    CheckEvent.Invoke();
                    this.enabled = false;
                }
                break;
        }
    }
}
