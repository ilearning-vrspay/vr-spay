using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StepCreationTool
{
    
    public class ReviewGroupGUI
    {
        // Start is called before the first frame update
        public string StepClass { get; set; }
        public StepGroupData stepGroupData { get; set; }

        public bool Foldout { get; set; } = true;
        private GUIStyle textStyle { get; set; }
        private readonly List<ReviewStepGUI> reviewStepGUIs = new List<ReviewStepGUI>();
        private readonly List<StepData> stepDataList = new List<StepData>();



        
        public void Render()
        {

            Foldout = EditorGUILayout.Foldout(Foldout, "HI");
            if (!Foldout)
            {
                return;
            }

            EditorGUILayout.Space(5.0f);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.Space(10.0f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("//-------", GUILayout.Width(50));
            Debug.Log("stepGroupData.groupName: " + stepGroupData.groupName);
            
            stepGroupData.groupName = EditorGUILayout.TextField( stepGroupData.groupName, GUILayout.Width(25));

            
            EditorGUILayout.LabelField("------//", GUILayout.Width(100));
            // if (GUILayout.Button("Propogate")){
            //     foreach (var step in group.stepList)
            //     {
            //         step.GroupName = StepGroupData.groupName;
            //     }
            // }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}