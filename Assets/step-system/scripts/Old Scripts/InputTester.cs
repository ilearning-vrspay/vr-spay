using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A monobehaviour for testing a function with a butotn press
/// 
/// Properties:
///     onSpacePressed: UnityEvent. Hook up any functions you want to test from the editor. 
///     
/// Methods:
///     Update: Checks to see if space is pressed to fire the onSpacePressed event. 
/// </summary>
public class InputTester : MonoBehaviour
{
    [SerializeField] private UnityEvent onSpacePressed = new UnityEvent();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSpacePressed.Invoke();
        }
    }
}
