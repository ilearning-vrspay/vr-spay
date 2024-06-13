using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public List<ExampleDS> ListOfExampleDS = new List<ExampleDS>();
}

[System.Serializable]
public class ExampleDS
{
    public GameObject Variation;
    public List<GameObject> Variations;
}
