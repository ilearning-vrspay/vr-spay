using UnityEngine;
using UnityEngine.Events;

public class KeyboardInputHandler : MonoBehaviour
{
    [System.Serializable]
    public class KeyEvent : UnityEvent { } // UnityEvent without parameters

    public KeyEvent onSpacePressed;
    public KeyEvent onAPressed;
    public KeyEvent onDPressed;

    void Update()
    {
        // Check for keyboard input in Update method
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSpacePressed.Invoke(); // Invoke the UnityEvent for Space key press
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            onAPressed.Invoke(); // Invoke the UnityEvent for A key press
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            onDPressed.Invoke(); // Invoke the UnityEvent for D key press
        }
    }
}