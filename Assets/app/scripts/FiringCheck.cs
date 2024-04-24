using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;


public class FiringCheck : MonoBehaviour
{
    public bool _firing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_firing)
        {
            Debug.Log("Firing");
        }
    }

    public void Fire()
    {
        _firing = !_firing;
        Debug.Log("Firing: " + _firing);
    }
}
