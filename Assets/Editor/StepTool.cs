using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;

public class StepTool : EditorWindow

{

    private bool renumberClipsEnabled = false;
    private int startingNumber = 1;
    private enum NumberingPosition { Prefix, Suffix }
    private NumberingPosition numberingPosition = NumberingPosition.Prefix;

    private string stepName = ""; // For naming the GameObject
    private string scriptContent = ""; // For the script content to populate in the component
    private string descriptionInput = "";
    private int selectedTypeIndex = 0;
    private string customTypeInput = "";
    private readonly List<string> types = new List<string>
    {
        "Dialogue", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
    };

    [MenuItem("Tools/Step Tool")]
    public static void ShowWindow()
    {
        GetWindow<StepTool>("Step Tool");
    }

    void OnGUI()
    {
        
        float width = EditorGUIUtility.currentViewWidth;

        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Step Tool", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        int numDashes = Mathf.FloorToInt(width / 6);
        string dashes = new string('-', numDashes);
        GUILayout.Label(dashes);
        GUILayout.Space(10f);

        GameObject selectedGameObject = Selection.activeGameObject;

        // Field to name the step or display the selected object's name
        stepName = EditorGUILayout.TextField("Step Name", stepName == "" && selectedGameObject != null ? selectedGameObject.name : stepName);

        // Field for the script content
        scriptContent = EditorGUILayout.TextField("Script Content", scriptContent);

        // Continue with your existing GUI for description
        descriptionInput = EditorGUILayout.TextField("Description", descriptionInput);
        selectedTypeIndex = EditorGUILayout.Popup("Type", selectedTypeIndex, types.ToArray());
        if (types[selectedTypeIndex] == "Custom")
        {
            customTypeInput = EditorGUILayout.TextField("Custom Type", customTypeInput);
        }

        // Check if the selected GameObject has an InstructionDeliveryController component
        bool hasControllerComponent = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() != null;

        // Adjust button label based on whether the selected GameObject has the component
        string buttonLabel = hasControllerComponent ? "Update Step" : "Create New Step";

        if (GUILayout.Button(buttonLabel))
        {
            if (!hasControllerComponent)
            {
                // Create new GameObject as a step and make it a child of the selected GameObject
                GameObject newStep = new GameObject(stepName);
                if (selectedGameObject != null)
                {
                    newStep.transform.SetParent(selectedGameObject.transform);
                }

                // Add the InstructionDeliveryController component and set up fields
                SetupStep(newStep);
            }
            else
            {
                // The selected GameObject already has the component, so update its fields
                SetupStep(selectedGameObject);
            }
        }
        // add a gui line to separate the renumber clips section from the rest of the tool 


        GUILayout.Space(10f);
        GUILayout.Label(dashes);
        GUILayout.Label("Renumber Clips", EditorStyles.boldLabel);
        renumberClipsEnabled = EditorGUILayout.Toggle("Enable Renumbering", renumberClipsEnabled);

        if (renumberClipsEnabled)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Starting Number:", GUILayout.Width(100));
            startingNumber = EditorGUILayout.IntField(startingNumber);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Renumber Clips"))
            {
                List<GameObject> selectedGameObjects = new List<GameObject>(Selection.gameObjects);
                selectedGameObjects.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

                int currentNumber = startingNumber;
                foreach (GameObject obj in selectedGameObjects)
                {
                    string originalName = obj.name;
                    string objSplit = originalName.Split('_')[0];
                    //check if objSplit is a number, if it's not a number, then just add the current number to the front of the original name with an underscore in between
                    if (!int.TryParse(objSplit, out int result))
                    {
                        obj.name = currentNumber + "_" + originalName;
                        currentNumber++;
                        continue;
                    }
                    
                    // Replace the number at the beginning with the new number
                    string newName = Regex.Replace(originalName, @"^\d+", currentNumber.ToString());
                    obj.name = newName;
                    currentNumber++;
                }
                
            }
        }
    }


    private void SetupStep(GameObject stepObject)
    {
        InstructionDeliveryController controller = stepObject.GetComponent<InstructionDeliveryController>();
        if (controller == null)
        {
            controller = stepObject.AddComponent<InstructionDeliveryController>();
        }

        // Create a SerializedObject that represents the component
        SerializedObject serializedController = new SerializedObject(controller);
        // Find the 'script' property, which is private and serialized
        SerializedProperty scriptProperty = serializedController.FindProperty("script");

        // Now update the 'script' property
        if (scriptProperty != null)
        {
            scriptProperty.stringValue = scriptContent; // Assign the new value
            serializedController.ApplyModifiedProperties(); // Apply changes to the actual component
        }

        // Similarly, you can directly update public fields or use SerializedProperty for private fields
        controller.description = descriptionInput;

        // If MetaData is accessible and modifiable like this
        if (controller.MetaData != null)
        {
            controller.MetaData.SetFileName(stepName);
        }

        GenerateAndApplyDescription(stepObject); // Apply your description formatting

        EditorUtility.SetDirty(controller); // Mark the component as dirty to ensure changes are saved
        // Mark the scene as dirty if the GameObject is part of the scene to save changes
        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(stepObject)))
        {
            EditorSceneManager.MarkSceneDirty(stepObject.scene);
        }

        if (types[selectedTypeIndex] == "Dialogue" && !string.IsNullOrEmpty(scriptContent))
        {
            // Call the GenerateAudio method from InstructionDeliveryControllerEditor
            InstructionDeliveryControllerEditor.GenerateAudio(controller, scriptContent);
        }
    }


    // Existing method for applying the description formatting
    private void GenerateAndApplyDescription(GameObject selectedGameObject)
    {
        if (selectedGameObject.TryGetComponent<InstructionDeliveryController>(out var controller))
        {
            string type = types[selectedTypeIndex] != "Custom" ? types[selectedTypeIndex].ToUpper() : customTypeInput.ToUpper();

            // Convert description and type to uppercase
            string descriptionText = $" {descriptionInput} ".ToUpper();
            string typeText = $" {type} ".ToUpper();

            // Calculate the maximum length needed for dashes based on the longest text
            int maxTextLength = Mathf.Max(descriptionText.Length, typeText.Length);
            int totalLength = maxTextLength + 8; // Adding padding for aesthetics

            // Create the dashed lines
            Debug.Log("totalLength: " + totalLength);
            //create an offset number based on totalLength
            int dashLineOffset = totalLength/8;

            string dashLine = new string('-', totalLength + dashLineOffset);

            // Center the description and type within the dashed lines
            string centeredDescription = CenterTextWithinDashes(descriptionText, totalLength);
            int typeLineOffset = totalLength/10;
            string centeredType = CenterTextWithinDashes(typeText, totalLength + typeLineOffset);

            // Construct the final formatted description
            controller.description = $"{centeredDescription}\n{dashLine}\n{centeredType}";

            // Mark the object as having been modified
            EditorUtility.SetDirty(controller);
        }
        else
        {
            Debug.LogError("The selected GameObject does not have an InstructionDeliveryController component.");
        }
    }

    // Existing method for centering text within dashes
    private string CenterTextWithinDashes(string text, int totalLength)
    {
        int padding = (totalLength - text.Length) / 2; // Calculate padding for each side
        string paddedText = new string('-', padding) + text + new string('-', totalLength - text.Length - padding);
        return paddedText;
    }

}
