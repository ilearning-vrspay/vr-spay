using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.Configuration;
using Mono.Reflection;

public class StepTool : EditorWindow

{

    private bool renumberClipsEnabled = false;
    private int startingNumber = 1;
    private enum NumberingPosition { Prefix, Suffix }
    private NumberingPosition numberingPosition = NumberingPosition.Prefix;

    private string groupName = "";
    private string stepName = ""; // For naming the GameObject
    private readonly List<string> types = new List<string>
    {
        "Dialogue", "Instruction", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
    };
    private string scriptContent = ""; // For the script content to populate in the component
    private string descriptionInput = "";
    private int selectedTypeIndex = 0;
    private string customTypeInput = "";
    

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
        groupName = EditorGUILayout.TextField("Group Name", groupName);
        stepName = EditorGUILayout.TextField("Step Name", stepName == "" && selectedGameObject != null ? selectedGameObject.name : stepName);
        selectedTypeIndex = EditorGUILayout.Popup("Type", selectedTypeIndex, types.ToArray());

        if (types[selectedTypeIndex] == "Custom")
                {
                    customTypeInput = EditorGUILayout.TextField("Custom Type", customTypeInput);
                }

        GUILayout.Space(10f);

        // Field for the script content
        scriptContent = EditorGUILayout.TextField("Script Content", scriptContent);

        // Continue with your existing GUI for description
        descriptionInput = EditorGUILayout.TextField("Description", descriptionInput);
        

        // Check if the selected GameObject has an InstructionDeliveryController component
        bool hasControllerComponent = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() != null;

        GUILayout.Space(10f);
        
        // Button for creating a new child step
        if (GUILayout.Button("Create New Step"))
        {
            
            string objectName = SetupFullName();
            
            GameObject newStep = new GameObject(objectName);
            if (selectedGameObject != null)
            {
                newStep.transform.SetParent(selectedGameObject.transform, false);
            }

            SetupStep(newStep, objectName);
        }

        // Button for updating the existing step, only shown if a suitable GameObject is selected
        if (hasControllerComponent)
        {
            if (GUILayout.Button("Update Step"))
            {
                SetupStep(selectedGameObject, SetupFullName(false));
            }
        }
        GUILayout.Label(dashes);
        GUILayout.Space(10f);
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
                RenumberClips();
            }
        }
    }

    private string SetupFullName(bool constructName = true){
        if (constructName == false)
        {
            return Selection.activeGameObject.name;
        }
        GameObject selectedGameObject = Selection.activeGameObject;

        string type = (types[selectedTypeIndex] != "Custom" ? types[selectedTypeIndex] : customTypeInput).Replace(" ", "");
        string baseName = $"{groupName}_{stepName}_{type}";
        
        GameObject parentObject = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() == null ? selectedGameObject : null;
            
        string prefix = DetermineStepPrefix(selectedGameObject, parentObject);
        string fullName = $"{prefix}_{baseName}";

        return fullName;
    }

    private string DetermineStepPrefix(GameObject selectedGameObject, GameObject parentObject = null)
    {
        // Check if we are adding as a child to an object with a controller
        if (selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() != null)
        {
            // We're adding a child step. Find the appropriate name based on siblings.
            return DetermineChildStepName(selectedGameObject);
        }
        else
        {
            // We're adding a new root step. Determine the next integer prefix.
            Debug.Log("DetermineNextRootStepName");
            return DetermineNextRootStepName(parentObject);
        }
    }

    // Placeholder for DetermineChildStepName - will implement next
    private string DetermineChildStepName(GameObject parent)
    {
        string parentName = parent.name;
        int number = 0;
        
        // Attempt to extract the numeric prefix from the parent's name
        Match match = Regex.Match(parentName, @"^(\d+)_");
        if (match.Success)
        {
            number = int.Parse(match.Groups[1].Value);
        }

        // Find the next available suffix letter for the new child
        char nextSuffix = 'a';
        foreach (Transform sibling in parent.transform)
        {
            string siblingName = sibling.name;
            // Check if sibling name follows the pattern "number + letter"
            Match siblingMatch = Regex.Match(siblingName, @"^" + number + @"([a-z])_");

            if (siblingMatch.Success)
            {
                char siblingSuffix = siblingMatch.Groups[1].Value[0];
                if (siblingSuffix >= nextSuffix)
                {
                    // Move to the next letter in the alphabet
                    nextSuffix = (char)(siblingSuffix + 1);
                }
            }
        }

        // Construct the new child name with the correct suffix letter
        return $"{number}{nextSuffix}";
    }



    // Placeholder for DetermineNextRootStepName - will implement next
    private string DetermineNextRootStepName(GameObject parent = null)
    {
        
        int highestNumber = 0;
        foreach (Transform childTransform in parent.transform)
        {
            GameObject obj = childTransform.gameObject;
            int currentNumber = ExtractLeadingNumber(obj.name);
            if (currentNumber > highestNumber)
            {
                highestNumber = currentNumber;
            }
        }
        return (highestNumber + 1).ToString(); // Increment and append a base name
    }




    private int ExtractLeadingNumber(string name)
    {
        var match = Regex.Match(name, @"^(\d+)_");
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }
        return 0; // No leading number found
    }



    // Consider moving the renumbering logic into its own method for clarity
    private void RenumberClips()
    {
        List<GameObject> selectedGameObjects = new List<GameObject>(Selection.gameObjects);
        selectedGameObjects.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

        int currentNumber = startingNumber;
        foreach (GameObject obj in selectedGameObjects)
        {
            string originalName = obj.name;
            // Determine if the original name starts with a number and replace it
            string newName = Regex.Replace(originalName, @"^\d+", currentNumber.ToString());
            if (newName == originalName) // No number at the start, prepend the number
            {
                newName = currentNumber + "_" + originalName;
            }
            obj.name = newName;
            currentNumber++;
        }
    }



    private void SetupStep(GameObject stepObject, string fullName)
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
            controller.MetaData.SetFileName(fullName);
        }

        GenerateAndApplyDescription(stepObject); // Apply your description formatting

        EditorUtility.SetDirty(controller); // Mark the component as dirty to ensure changes are saved
        // Mark the scene as dirty if the GameObject is part of the scene to save changes
        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(stepObject)))
        {
            EditorSceneManager.MarkSceneDirty(stepObject.scene);
        }

        if (types[selectedTypeIndex] == "Dialogue" || types[selectedTypeIndex] == "Instruction" && !string.IsNullOrEmpty(scriptContent))
        {
            // Call the GenerateAudio method from InstructionDeliveryControllerEditor
            InstructionDeliveryControllerEditor.GenerateAudio(controller, scriptContent);
            InstructionDeliveryControllerEditor.GenerateTimeline(controller);
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
