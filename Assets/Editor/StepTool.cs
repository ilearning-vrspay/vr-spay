// using UnityEngine;
// using UnityEditor;
// using Xceed.Words.NET; // Import DocX library
// using System.IO;
// using System;
// using System.Collections.Generic;
// using UnityEditor.SceneManagement;
// using System.Text.RegularExpressions;
// using System.Configuration;
// using Mono.Reflection;

// public class StepTool : EditorWindow

// {
//     private string docxFilePath = ""; // Path to the .docx file
//     private string groupName = ""; // Current group name

//     private bool renumberClipsEnabled = false;
//     private int startingNumber = 1;
//     private enum NumberingPosition { Prefix, Suffix }
//     private NumberingPosition numberingPosition = NumberingPosition.Prefix;

//     private string groupName = "";
//     private string stepName = ""; // For naming the GameObject
//     private readonly List<string> types = new List<string>
//     {
//         "Dialogue", "Instruction", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
//     };
//     private string scriptContent = ""; // For the script content to populate in the component
//     private string descriptionInput = "";
//     private int selectedTypeIndex = 0;
//     private string customTypeInput = "";
    

//     [MenuItem("Tools/Step Tool")]
//     public static void ShowWindow()
//     {
//         GetWindow<StepTool>("Step Tool");
//     }

//    void OnGUI()
//     {
//         float width = EditorGUIUtility.currentViewWidth;

//         GUILayout.Space(10f);
//         GUILayout.BeginHorizontal();
//         GUILayout.FlexibleSpace();
//         GUILayout.Label("Step Tool", EditorStyles.boldLabel);
//         GUILayout.FlexibleSpace();
//         GUILayout.EndHorizontal();

//         int numDashes = Mathf.FloorToInt(width / 6);
//         string dashes = new string('-', numDashes);
//         GUILayout.Label(dashes);
//         GUILayout.Space(10f);

//         GameObject selectedGameObject = Selection.activeGameObject;

//         // Field to name the step or display the selected object's name
//         groupName = EditorGUILayout.TextField("Group Name", groupName);
//         stepName = EditorGUILayout.TextField("Step Name", stepName == "" && selectedGameObject != null ? selectedGameObject.name : stepName);
//         selectedTypeIndex = EditorGUILayout.Popup("Type", selectedTypeIndex, types.ToArray());

//         if (types[selectedTypeIndex] == "Custom")
//                 {
//                     customTypeInput = EditorGUILayout.TextField("Custom Type", customTypeInput);
//                 }

//         GUILayout.Space(10f);

//         // Field for the script content
//         scriptContent = EditorGUILayout.TextField("Script Content", scriptContent);

//         // Continue with your existing GUI for description
//         descriptionInput = EditorGUILayout.TextField("Description", descriptionInput);
        

//         // Check if the selected GameObject has an InstructionDeliveryController component
//         bool hasControllerComponent = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() != null;

//         GUILayout.Space(10f);
        
//         // Button for creating a new child step
//         if (GUILayout.Button("Create New Step"))
//         {
            
//             string objectName = SetupFullName();
            
//             GameObject newStep = new GameObject(objectName);
//             if (selectedGameObject != null)
//             {
//                 newStep.transform.SetParent(selectedGameObject.transform, false);
//             }

//             SetupStep(newStep, objectName);
//         }

//         // Button for updating the existing step, only shown if a suitable GameObject is selected
//         if (hasControllerComponent)
//         {
//             if (GUILayout.Button("Update Step"))
//             {
//                 SetupStep(selectedGameObject, SetupFullName(false));
//             }
//         }
//         GUILayout.Label(dashes);
//         GUILayout.Space(10f);
//         GUILayout.Label("Renumber Clips", EditorStyles.boldLabel);
//         renumberClipsEnabled = EditorGUILayout.Toggle("Enable Renumbering", renumberClipsEnabled);

//         if (renumberClipsEnabled)
//         {
//             GUILayout.BeginHorizontal();
//             GUILayout.Label("Starting Number:", GUILayout.Width(100));
//             startingNumber = EditorGUILayout.IntField(startingNumber);
//             GUILayout.EndHorizontal();

//             if (GUILayout.Button("Renumber Clips"))
//             {
//                 RenumberClips();
//             }
//         }
//     }

//     private string SetupFullName(bool constructName = true){
//         if (constructName == false)
//         {
//             return Selection.activeGameObject.name;
//         }
//         GameObject selectedGameObject = Selection.activeGameObject;

//         string type = (types[selectedTypeIndex] != "Custom" ? types[selectedTypeIndex] : customTypeInput).Replace(" ", "");
//         string baseName = $"{groupName}_{stepName}_{type}";
        
//         GameObject parentObject = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() == null ? selectedGameObject : null;
            
//         string prefix = DetermineStepPrefix(selectedGameObject, parentObject);
//         string fullName = $"{prefix}_{baseName}";

//         return fullName;
//     }

//     private string DetermineStepPrefix(GameObject selectedGameObject, GameObject parentObject = null)
//     {
//         // Check if we are adding as a child to an object with a controller
//         if (selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() != null)
//         {
//             // We're adding a child step. Find the appropriate name based on siblings.
//             return DetermineChildStepName(selectedGameObject);
//         }
//         else
//         {
//             // We're adding a new root step. Determine the next integer prefix.
//             Debug.Log("DetermineNextRootStepName");
//             return DetermineNextRootStepName(parentObject);
//         }
//     }

//     // Placeholder for DetermineChildStepName - will implement next
//     private string DetermineChildStepName(GameObject parent)
//     {
//         string parentName = parent.name;
//         int number = 0;
        
//         // Attempt to extract the numeric prefix from the parent's name
//         Match match = Regex.Match(parentName, @"^(\d+)_");
//         if (match.Success)
//         {
//             number = int.Parse(match.Groups[1].Value);
//         }

//         // Find the next available suffix letter for the new child
//         char nextSuffix = 'a';
//         foreach (Transform sibling in parent.transform)
//         {
//             string siblingName = sibling.name;
//             // Check if sibling name follows the pattern "number + letter"
//             Match siblingMatch = Regex.Match(siblingName, @"^" + number + @"([a-z])_");

//             if (siblingMatch.Success)
//             {
//                 char siblingSuffix = siblingMatch.Groups[1].Value[0];
//                 if (siblingSuffix >= nextSuffix)
//                 {
//                     // Move to the next letter in the alphabet
//                     nextSuffix = (char)(siblingSuffix + 1);
//                 }
//             }
//         }

//         // Construct the new child name with the correct suffix letter
//         return $"{number}{nextSuffix}";
//     }



//     // Placeholder for DetermineNextRootStepName - will implement next
//     private string DetermineNextRootStepName(GameObject parent = null)
//     {
        
//         int highestNumber = 0;
//         foreach (Transform childTransform in parent.transform)
//         {
//             GameObject obj = childTransform.gameObject;
//             int currentNumber = ExtractLeadingNumber(obj.name);
//             if (currentNumber > highestNumber)
//             {
//                 highestNumber = currentNumber;
//             }
//         }
//         return (highestNumber + 1).ToString(); // Increment and append a base name
//     }




//     private int ExtractLeadingNumber(string name)
//     {
//         var match = Regex.Match(name, @"^(\d+)_");
//         if (match.Success)
//         {
//             return int.Parse(match.Groups[1].Value);
//         }
//         return 0; // No leading number found
//     }



//     // Consider moving the renumbering logic into its own method for clarity
//     private void RenumberClips()
//     {
//         List<GameObject> selectedGameObjects = new List<GameObject>(Selection.gameObjects);
//         selectedGameObjects.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));

//         int currentNumber = startingNumber;
//         foreach (GameObject obj in selectedGameObjects)
//         {
//             string originalName = obj.name;
//             // Determine if the original name starts with a number and replace it
//             string newName = Regex.Replace(originalName, @"^\d+", currentNumber.ToString());
//             if (newName == originalName) // No number at the start, prepend the number
//             {
//                 newName = currentNumber + "_" + originalName;
//             }
//             obj.name = newName;
//             currentNumber++;
//         }
//     }



//     private void SetupStep(GameObject stepObject, string fullName)
//     {
//         InstructionDeliveryController controller = stepObject.GetComponent<InstructionDeliveryController>();
//         if (controller == null)
//         {
//             controller = stepObject.AddComponent<InstructionDeliveryController>();
//         }

//         // Create a SerializedObject that represents the component
//         SerializedObject serializedController = new SerializedObject(controller);
//         // Find the 'script' property, which is private and serialized
//         SerializedProperty scriptProperty = serializedController.FindProperty("script");

//         // Now update the 'script' property
//         if (scriptProperty != null)
//         {
//             scriptProperty.stringValue = scriptContent; // Assign the new value
//             serializedController.ApplyModifiedProperties(); // Apply changes to the actual component
//         }

//         // Similarly, you can directly update public fields or use SerializedProperty for private fields
//         controller.description = descriptionInput;

//         // If MetaData is accessible and modifiable like this
//         if (controller.MetaData != null)
//         {
//             controller.MetaData.SetFileName(fullName);
//         }

//         GenerateAndApplyDescription(stepObject); // Apply your description formatting

//         EditorUtility.SetDirty(controller); // Mark the component as dirty to ensure changes are saved
//         // Mark the scene as dirty if the GameObject is part of the scene to save changes
//         if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(stepObject)))
//         {
//             EditorSceneManager.MarkSceneDirty(stepObject.scene);
//         }

//         if (types[selectedTypeIndex] == "Dialogue" || types[selectedTypeIndex] == "Instruction" && !string.IsNullOrEmpty(scriptContent))
//         {
//             // Call the GenerateAudio method from InstructionDeliveryControllerEditor
//             InstructionDeliveryControllerEditor.GenerateAudio(controller, scriptContent);
//             InstructionDeliveryControllerEditor.GenerateTimeline(controller);
//         }
//     }


//     // Existing method for applying the description formatting
//     private void GenerateAndApplyDescription(GameObject selectedGameObject)
//     {
//         if (selectedGameObject.TryGetComponent<InstructionDeliveryController>(out var controller))
//         {
//             string type = types[selectedTypeIndex] != "Custom" ? types[selectedTypeIndex].ToUpper() : customTypeInput.ToUpper();

//             // Convert description and type to uppercase
//             string descriptionText = $" {descriptionInput} ".ToUpper();
//             string typeText = $" {type} ".ToUpper();

//             // Calculate the maximum length needed for dashes based on the longest text
//             int maxTextLength = Mathf.Max(descriptionText.Length, typeText.Length);
//             int totalLength = maxTextLength + 8; // Adding padding for aesthetics

//             // Create the dashed lines
//             Debug.Log("totalLength: " + totalLength);
//             //create an offset number based on totalLength
//             int dashLineOffset = totalLength/8;

//             string dashLine = new string('-', totalLength + dashLineOffset);

//             // Center the description and type within the dashed lines
//             string centeredDescription = CenterTextWithinDashes(descriptionText, totalLength);
//             int typeLineOffset = totalLength/10;
//             string centeredType = CenterTextWithinDashes(typeText, totalLength + typeLineOffset);

//             // Construct the final formatted description
//             controller.description = $"{centeredDescription}\n{dashLine}\n{centeredType}";

//             // Mark the object as having been modified
//             EditorUtility.SetDirty(controller);
//         }
//         else
//         {
//             Debug.LogError("The selected GameObject does not have an InstructionDeliveryController component.");
//         }
//     }

//     // Existing method for centering text within dashes
//     private string CenterTextWithinDashes(string text, int totalLength)
//     {
//         int padding = (totalLength - text.Length) / 2; // Calculate padding for each side
//         string paddedText = new string('-', padding) + text + new string('-', totalLength - text.Length - padding);
//         return paddedText;
//     }

//     private void ReadDocxFile(string filePath)
//     {
//         if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
//         {
//             Debug.LogError("Invalid file path.");
//             return;
//         }

//         int retryCount = 3;
//         int retryDelay = 1000; // 1 second

//         for (int i = 0; i < retryCount; i++)
//         {
//             try
//             {
//                 using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//                 {
//                     using (DocX document = DocX.Load(fs))
//                     {
//                         Debug.Log("Reading document...");
//                         foreach (var paragraph in document.Paragraphs)
//                         {
//                             string text = paragraph.Text.Trim();
//                             if (string.IsNullOrEmpty(text)) continue;

//                             // Check for group name
//                             if (text.StartsWith("**") && text.EndsWith("**"))
//                             {
//                                 groupName = text.Substring(2, text.Length - 4).Trim();
//                                 Debug.Log($"Group Name: {groupName}");
//                                 continue;
//                             }

//                             Debug.Log($"Full Paragraph: {text}");

//                             string stepName = ExtractBetween(text, "{", "}");
//                             string stepType = ExtractBetween(text, "[", "]");
//                             string scriptContent = ExtractBetween(text, "`", "`");

//                             if (!string.IsNullOrEmpty(stepName) && !string.IsNullOrEmpty(stepType))
//                             {
//                                 Debug.Log($"Step Name: {stepName}");
//                                 Debug.Log($"Step Type: {stepType}");
//                                 Debug.Log($"Script Content: {scriptContent}");
//                             }
//                             else if (stepType == "user action")
//                             {
//                                 // Treat as a user action if tagged with [user action]
//                                 Debug.Log($"User Action: {text}");
//                             }
//                         }
//                     }
//                 }
//                 break; // Exit the loop if successful
//             }
//             catch (IOException ioEx)
//             {
//                 Debug.LogError($"IOException: {ioEx.Message}");
//                 if (i < retryCount - 1)
//                 {
//                     Debug.Log("Retrying...");
//                     System.Threading.Thread.Sleep(retryDelay); // Wait before retrying
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to read the file after multiple attempts.");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Exception: {ex.Message}");
//                 break;
//             }
//         }
//     }

//     private string ExtractBetween(string text, string start, string end)
//     {
//         var escapedStart = Regex.Escape(start);
//         var escapedEnd = Regex.Escape(end);

//         var match = Regex.Match(text, $@"{escapedStart}(.*?){escapedEnd}");
//         Debug.Log($"Extracting between {start} and {end}: Found '{match.Groups[1].Value}' in '{text}'");
//         return match.Success ? match.Groups[1].Value : string.Empty;
//     }

// }
using UnityEngine;
using UnityEditor;
using Xceed.Words.NET; // Import DocX library
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.Configuration;
using Mono.Reflection;

public class StepTool : EditorWindow
{
    private string docxFilePath = ""; // Path to the .docx file
    private string currentGroupName = ""; // Current group name

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

    private List<StepData> stepDataList = new List<StepData>(); // List to hold parsed step data
    private GameObject lastCreatedStep = null; // Track the last created step

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

        GUILayout.Space(10f);
        GUILayout.Label("Docx Import", EditorStyles.boldLabel);
        docxFilePath = EditorGUILayout.TextField("Docx File Path", docxFilePath);

        if (GUILayout.Button("Import Steps from Docx"))
        {
            ImportStepsFromDocx(docxFilePath);
        }
    }

    private string SetupFullName(bool constructName = true)
    {
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
            int dashLineOffset = totalLength / 8;

            string dashLine = new string('-', totalLength + dashLineOffset);

            // Center the description and type within the dashed lines
            string centeredDescription = CenterTextWithinDashes(descriptionText, totalLength);
            int typeLineOffset = totalLength / 10;
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

    private string CenterTextWithinDashes(string text, int totalLength)
    {
        int padding = (totalLength - text.Length) / 2; // Calculate padding for each side
        string paddedText = new string('-', padding) + text + new string('-', totalLength - text.Length - padding);
        return paddedText;
    }

    // New method to import steps from a .docx file
    private void ImportStepsFromDocx(string filePath)
    {
        ReadDocxFile(filePath);
        foreach (var stepData in stepDataList)
        {
            CreateStep(stepData);
        }
    }

    private void ReadDocxFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("Invalid file path.");
            return;
        }

        stepDataList.Clear(); // Clear the existing data

        int retryCount = 3;
        int retryDelay = 1000; // 1 second

        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (DocX document = DocX.Load(fs))
                    {
                        Debug.Log("Reading document...");
                        foreach (var paragraph in document.Paragraphs)
                        {
                            string text = paragraph.Text.Trim();
                            if (string.IsNullOrEmpty(text)) continue;

                            // Check for group name
                            if (text.StartsWith("**") && text.EndsWith("**"))
                            {
                                currentGroupName = text.Substring(2, text.Length - 4).Trim();
                                Debug.Log($"Group Name: {currentGroupName}");
                                continue;
                            }

                            Debug.Log($"Full Paragraph: {text}");

                            string stepName = ExtractBetween(text, "{", "}");
                            string stepType = ExtractBetween(text, "[", "]");
                            string scriptContent = ExtractBetween(text, "`", "`");

                            if (!string.IsNullOrEmpty(stepName) && !string.IsNullOrEmpty(stepType))
                            {
                                Debug.Log($"Step Name: {stepName}");
                                Debug.Log($"Step Type: {stepType}");
                                Debug.Log($"Script Content: {scriptContent}");

                                stepDataList.Add(new StepData
                                {
                                    GroupName = currentGroupName,
                                    StepName = stepName,
                                    StepType = stepType,
                                    ScriptContent = scriptContent,
                                    Description = stepName // Only the step name for the description
                                });
                            }
                            else if (stepType == "user action")
                            {
                                // Treat as a user action if tagged with [user action]
                                Debug.Log($"User Action: {text}");
                                stepDataList.Add(new StepData
                                {
                                    GroupName = currentGroupName,
                                    StepName = "UserAction",
                                    StepType = "User Action",
                                    ScriptContent = text,
                                    Description = "UserAction" // Description for user action steps
                                });
                            }
                        }
                    }
                }
                break; // Exit the loop if successful
            }
            catch (IOException ioEx)
            {
                Debug.LogError($"IOException: {ioEx.Message}");
                if (i < retryCount - 1)
                {
                    Debug.Log("Retrying...");
                    System.Threading.Thread.Sleep(retryDelay); // Wait before retrying
                }
                else
                {
                    Debug.LogError("Failed to read the file after multiple attempts.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
                break;
            }
        }
    }

    private string ExtractBetween(string text, string start, string end)
    {
        var escapedStart = Regex.Escape(start);
        var escapedEnd = Regex.Escape(end);

        var match = Regex.Match(text, $@"{escapedStart}(.*?){escapedEnd}");
        Debug.Log($"Extracting between {start} and {end}: Found '{match.Groups[1].Value}' in '{text}'");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private void CreateStep(StepData stepData)
    {
        groupName = stepData.GroupName;
        stepName = stepData.StepName;

        int typeIndex = types.IndexOf(stepData.StepType);
        if (typeIndex >= 0)
        {
            selectedTypeIndex = typeIndex;
        }
        else
        {
            selectedTypeIndex = types.IndexOf("Custom");
            customTypeInput = stepData.StepType;
        }

        scriptContent = stepData.ScriptContent;
        descriptionInput = stepData.Description;

        // Determine the parent for the new step
        GameObject selectedGameObject = stepData.StepType == "User Action" && lastCreatedStep != null
            ? lastCreatedStep
            : Selection.activeGameObject;

        // Set the type index for user action
        if (stepData.StepType == "User Action")
        {
            selectedTypeIndex = types.IndexOf("User Action");
        }

        string objectName = SetupFullName();

        GameObject newStep = new GameObject(objectName);
        if (selectedGameObject != null)
        {
            newStep.transform.SetParent(selectedGameObject.transform, false);
        }

        SetupStep(newStep, objectName);

        // Update the last created step if it's not a user action
        if (stepData.StepType != "User Action")
        {
            lastCreatedStep = newStep;
        }
    }
}

// Data class to hold step information
public class StepData
{
    public string GroupName { get; set; }
    public string StepName { get; set; }
    public string StepType { get; set; }
    public string ScriptContent { get; set; }
    public string Description { get; set; }
}
