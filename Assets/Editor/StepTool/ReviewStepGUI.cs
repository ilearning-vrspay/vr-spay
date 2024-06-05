using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StepCreationTool
{
    
    public class ReviewStepGUI
    {
        // Start is called before the first frame update
        public string StepClass { get; set; }
        public StepData StepData { get; set; }
        public bool Foldout { get; set; } = true;
        private GUIStyle textStyle { get; set; }

        public void Render()
        {

            Foldout = EditorGUILayout.Foldout(Foldout, StepData.StepName);
            if (!Foldout)
            {
                return;
            }

            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step Name baby");
            EditorGUILayout.EndHorizontal();
        }
    }
}