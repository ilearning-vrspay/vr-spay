using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace StepCreationTool
{
    public class StepBuilder : EditorWindow
    {
        private StepGUI stepGUI = new StepGUI();
        public List<StepGroupData> stepCommandList;
        

        [MenuItem("Tools/Step Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<StepBuilder>("Step Builder");
            window.Initialize();
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


        void Initialize()
        {
            stepGUI.StepBuilder = this;
        }

        public void SetupStepsByDocx(string path)
        {
            stepCommandList = DocReader.ReadDocxFile(path);

            StepReviewWindow.ShowWindow(stepCommandList);

        }

        public void TestLogic()
        {
            Debug.Log("Testing Content");
        }
        

        void OnGUI()
        {
            float width = EditorGUIUtility.currentViewWidth;
            stepGUI.OnGUI(width);
            //if statement to check if a game object is selected
            InstructionDeliveryListController instructionDeliveryListController = Selection.activeGameObject?.GetComponent<InstructionDeliveryListController>();
            InstructionDeliveryController instructionDeliveryController = Selection.activeGameObject?.GetComponent<InstructionDeliveryController>();
            StepGroup stepGroup = Selection.activeGameObject?.GetComponent<StepGroup>();

            string generatedName = Selection.activeGameObject != null ? NameGenerator.SetupFullName(stepGUI) : null;
            // Debug.Log("Generated Name: " + generatedName);  
            

            if (instructionDeliveryListController != null || stepGroup != null) {

                if (GUILayout.Button("Create Step"))
                {
                    GameObject newStep = new GameObject(generatedName);

                    newStep.transform.SetParent(Selection.activeGameObject.transform, false);

                    // ObjectGenerator.CreateGameObject(generatedName);
                }

                if (instructionDeliveryListController != null)
                {
                    if (GUILayout.Button("Create Step Group"))
                    {
                        GameObject stepGroupObject = new GameObject("//----- New Step Group -----//");
                        stepGroupObject.transform.SetParent(Selection.activeGameObject.transform, false);
                        stepGroupObject.AddComponent<StepGroup>();
                    }
                }
            } else if (instructionDeliveryController != null) {
                if (GUILayout.Button("Create User Step"))
                {
                    GameObject newStep = new GameObject(generatedName);

                    newStep.transform.SetParent(Selection.activeGameObject.transform, false);
                    // ObjectGenerator.CreateUserStepObject(stepGUI);
                } 
            } else {
                EditorGUILayout.HelpBox("Select Step Sequence Object to create new step or Select Step Object to create user step", MessageType.Warning);
            }
            
        
        }
    }
}