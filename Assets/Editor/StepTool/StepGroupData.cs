using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StepCreationTool
{
    public class StepGroupData
    {
        public string groupName;
        public List<StepData> stepList = new List<StepData>();
    

        public void addStep(StepData stepData)
        {
            stepList.Add(stepData);
        }
    }

    
    
}
