using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepCreationTool
{
    public class Group
    {
        
        public StepGroupData stepGroupData { get; set; }

        public ReviewGroupGUI reviewGroupGUI { get; set; }

        public void Render()
        {
            reviewGroupGUI.Render();
        }
    


    }
}
