using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepCreationTool
{
public class UserStepData 
    {
        public string SequenceNumber { get; set; }

        public string GroupName { get; set; }
        public string StepName { get; set; }
        public string StepType { get; set; }
        public int StepTypeIndex { get; set; }
        public string CustomTypeInput { get; set; }
        public string ScriptContent { get; set; }
        public string Description { get; set; }
    }
}
