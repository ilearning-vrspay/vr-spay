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
            // Debug.Log("Listening");
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
            // Debug.Log("sending good vibes bro: " + value);
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
}
