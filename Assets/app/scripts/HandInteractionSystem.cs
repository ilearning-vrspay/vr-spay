using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandInteractionSystem : StandardHandAnimationComponent
{
    public ToolManager toolManager;
    [System.Serializable]
    public class InputActionBinding
    {

        public enum InputType { Button, Axis } 

        public InputType inputType;
        public InputActionReference inputActionReference;
        public UnityEvent<bool> onInputReceived;
        public UnityEvent<float> onContinuousInputReceived;
        


        public void StartListening()
        {
            InputAction action = inputActionReference.action;
            Debug.Log("Listening");
            switch (inputType)
            {
                case InputType.Button:
                    action.started += context => OnInputPerformed(context.ReadValue<float>() > 0.5f);
                    break;
                case InputType.Axis:
                    action.performed += context => OnContinuousInputPerformed(context.ReadValue<float>());
                    action.canceled += context => OnContinuousInputPerformed(context.ReadValue<float>());
                    break;
            }

        }

        public void OnInputPerformed(bool value)
        {
            onInputReceived.Invoke(value);
            

        }

        public void OnContinuousInputPerformed(float value)
        {
            Debug.Log("sending good vibes bro: " + value);
            onContinuousInputReceived.Invoke(value);
        }


        
    }

    public InputActionBinding[] inputActionBindings;


    override public void Start()
        {
            foreach (var inputActionBinding in inputActionBindings)
            {
                inputActionBinding.StartListening();
            }
            
        }
























    // OLD VERSION //




    // [System.Serializable]
    // public class InputActionBinding
    // {
    //     public InputActionReference inputActionReference;
    //     public InputHelpers.Button inputButton;

    //     public UnityEvent onInputReceived;

    //     [HideInInspector]
    //     public bool isActionTriggered = false;

    //     public void StartListening()
    //     {
    //         InputAction action = inputActionReference.action;

    //         action.started += context => OnInputPerformed();
    //     }

    //     private void OnInputPerformed()
    //     {
    //         Debug.Log("Input action performed: " + inputButton.ToString());
    //     }

    // }

    // // private string isTriggered = null;

    // [SerializeField]
    // private List<InputActionBinding> inputActions = new List<InputActionBinding>();
    // // Start is called before the first frame update

    // void Start()
    // {
    //     InputActionBinding inputBinding = new InputActionBinding();
    //     inputBinding.inputActionReference = 
    //     // base.Start();
    // }

    // // Update is called once per frame
    // void Update()
    // {

    //     // base.Update();

    //     // foreach (var inputAction in inputActions)
    //     // {

    //     //     if (targetDevice.IsPressed(inputAction.inputButton, out bool isPressed) && isPressed)
    //     //     {
    //     //         inputAction.onInputReceived.Invoke(); // Invokes the UnityEvent for the action
    //     //     }
    //     // }
    // }

    

}
