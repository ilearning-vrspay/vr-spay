using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace StepCreationTool
{
    public class StepGUI
    {
        public StepBuilder StepBuilder { get; set; }
        
        public string GroupName { get; set; } = "";
        public string StepName { get; set; } = "";
        public int SelectedTypeIndex { get; set; } = 0;
        public string CustomTypeInput { get; set; } = "";
        public string ScriptContent { get; set; } = "";
        public string DescriptionInput { get; set; } = "";
        public string DocxFilePath { get; set; } = "Default";
        public bool RenumberClipsEnabled { get; set; } = false;
        public int StartingNumber { get; set; } = 1;

        private readonly List<string> types = new List<string>
        {
            "Dialogue", "Instruction", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
        };

        
        public void OnGUI(float width)
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Step Builder", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            int numDashes = Mathf.FloorToInt(width / 6);
            string dashes = new string('-', numDashes);
            GUILayout.Label(dashes);
            GUILayout.Space(10f);


            // Field to name the step or display the selected object's name
            GroupName = EditorGUILayout.TextField("Group Name", GroupName);
            StepName = EditorGUILayout.TextField("Step Name", StepName);
            SelectedTypeIndex = EditorGUILayout.Popup("Type", SelectedTypeIndex, types.ToArray());

            if (types[SelectedTypeIndex] == "Custom")
            {
                CustomTypeInput = EditorGUILayout.TextField("Custom Type", CustomTypeInput);
            }

            GUILayout.Space(10f);

            // Field for the script content
            ScriptContent = EditorGUILayout.TextField("Script Content", ScriptContent);

            // GUI for description
            DescriptionInput = EditorGUILayout.TextField("Description", DescriptionInput);

            GUILayout.Space(10f);

            GUILayout.Label(dashes);
            GUILayout.Space(10f);
            GUILayout.Label("Renumber Clips", EditorStyles.boldLabel);
            RenumberClipsEnabled = EditorGUILayout.Toggle("Enable Renumbering", RenumberClipsEnabled);

            if (RenumberClipsEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Starting Number:", GUILayout.Width(100));
                StartingNumber = EditorGUILayout.IntField(StartingNumber);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10f);
            GUILayout.Label("Docx Import", EditorStyles.boldLabel);
            DocxFilePath = EditorGUILayout.TextField("Docx File Path", DocxFilePath);

            if (GUILayout.Button("Import Steps from Docx"))
            {
                if (DocxFilePath == "Default")
                {
                    DocxFilePath = "/Users/carterboyce/Dropbox/ScissorsGroup_Metzenbaum.docx";
                }
                if (DocxFilePath != "Default")
                {
                Debug.Log("Importing docx file...");
                

                StepBuilder.SetupStepsByDocx(DocxFilePath);
                } else
                {
                    Debug.LogError("No file path specified");
                }


            }

            if (GUILayout.Button("Test Name Generation"))
            {
                StepBuilder.TestLogic();
            }

        }
    }
}
