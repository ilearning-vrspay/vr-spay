using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{

    public List<InstructionDeliveryListController> instructionDeliveryListControllers;

    public int startingSequenceIndex = 0;
    private int currentSequenceIndex = 0;
    public int CurrentSequenceIndex => currentSequenceIndex;

    
   

    


    // Start is called before the first frame update
    void Start()
    {
        currentSequenceIndex = startingSequenceIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFullSequence() 
    {
        instructionDeliveryListControllers[currentSequenceIndex].StartSequence();
    
    }

    public void BeginNextSequence()
    {
        if (currentSequenceIndex < instructionDeliveryListControllers.Count)
        {
            currentSequenceIndex++;
            
            instructionDeliveryListControllers[currentSequenceIndex].StartSequence();
             
        }
    
    }

    public void SkipSequenceInstructions()
    {
        instructionDeliveryListControllers[currentSequenceIndex].SkipInstructions();
    }

    public void StartSequenceNextStep()
    {

        instructionDeliveryListControllers[currentSequenceIndex].StartNextStep();
    }
}
