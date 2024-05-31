using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StepCreationTool
{
    public class StepReviewWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        public List<StepGroupData> stepCommandList { get; set; }
        private GUIStyle textStyle;

        
        public static void ShowWindow(List<StepGroupData> list)
        {
            var window = GetWindow<StepReviewWindow>("Step Review");
            window.stepCommandList = list; 
            AssemblyReloadEvents.beforeAssemblyReload += window.OnBeforeAssemblyReload;
            EditorApplication.quitting += window.OnEditorQuitting;
        }

        private void OnBeforeAssemblyReload()
        {
            Close();
        }

        private void OnEditorQuitting()
        {
            Close();
        }

        private void OnDestroy()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            EditorApplication.quitting -= OnEditorQuitting;
        }

        private void OnEnable()
        {
            textStyle = new GUIStyle();
            textStyle.fontSize = 12;
            textStyle.normal.textColor = Color.white;
        }



    void OnGUI(){

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        int stepNumber = 1;
        EditorGUILayout.Space(10.0f);

        EditorGUILayout.LabelField("{StepNumber}_{StepSGroup}_{StepName}_{StepType}");

        foreach (var group in stepCommandList)
        {

            EditorGUILayout.Space(5.0f);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.Space(10.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("//-------", GUILayout.Width(50));
            Vector2 size = textStyle.CalcSize(new GUIContent(group.groupName));
            if (size.x < 50){
                size.x = 50;
            } else {
                size.x += 5;
            }
            
            group.groupName = EditorGUILayout.TextField( group.groupName, GUILayout.Width(size.x + 5));

            
            EditorGUILayout.LabelField("------//", GUILayout.Width(100));
            if (GUILayout.Button("Propogate")){
                foreach (var step in group.stepList)
                {
                    step.GroupName = group.groupName;
                }
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("  |");



            foreach (var step in group.stepList)
            {   
                EditorGUILayout.Space(5.0f);

                


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  |-- ", GUILayout.Width(30));

                Vector2 numSize = textStyle.CalcSize(new GUIContent(stepNumber.ToString()));
                if (numSize.x < 20){
                    numSize.x = 20;
                } else {
                    numSize.x += 5;
                }
                step.SequenceNumber = EditorGUILayout.IntField("", step.SequenceNumber, GUILayout.Width(numSize.x), GUILayout.ExpandWidth(true));
                
                EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));
                step.GroupName = EditorGUILayout.TextField( step.GroupName, GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));  
                step.StepName = EditorGUILayout.TextField( step.StepName, GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));
                step.StepType = EditorGUILayout.TextField(step.StepType, GUILayout.ExpandWidth(true));


                EditorGUILayout.EndHorizontal();


                var UserIdx = 0;
                foreach (var userStep in step.userSteps)
                {
                    EditorGUILayout.Space(5.0f);
                    
                    // var userStepChar = ((char)('a' + UserIdx)).ToString();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("  |", GUILayout.Width(30));
                    EditorGUILayout.LabelField("  |-- ", GUILayout.Width(30));
                    EditorGUILayout.TextField("", userStep.SequenceNumber, GUILayout.MaxWidth(25), GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));
                    EditorGUILayout.TextField( userStep.GroupName, GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));  
                    EditorGUILayout.TextField( userStep.StepName, GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("_", GUILayout.Width(10), GUILayout.ExpandWidth(true));
                    EditorGUILayout.TextField(userStep.StepType, GUILayout.ExpandWidth(true));


                    EditorGUILayout.EndHorizontal();
                    UserIdx++;
                
                }

                stepNumber++;




            }
        }
        EditorGUILayout.EndScrollView();

    }

        
    }
}