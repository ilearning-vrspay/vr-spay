using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StepDescriptionTool : EditorWindow
{
    private string descriptionInput = "";
    private int selectedTypeIndex = 0;
    private string customTypeInput = "";
    private readonly List<string> types = new List<string>
    {
        "Dialogue", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
    };

    // Add menu named "Step Description Tool" to the Tools menu
    [MenuItem("Tools/Step Description Tool")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(StepDescriptionTool));
    }

    void OnGUI()
    {
        GUILayout.Label("Step Description Generator", EditorStyles.boldLabel);

        // Automatically use the currently selected GameObject in the Editor
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject != null)
        {
            EditorGUILayout.LabelField("Selected GameObject:", selectedGameObject.name);
        }
        else
        {
            EditorGUILayout.HelpBox("No GameObject selected. Please select a GameObject in the scene.", MessageType.Warning);
        }

        // Input field for the description part
        descriptionInput = EditorGUILayout.TextField("Description", descriptionInput);

        // Dropdown for selecting the type
        selectedTypeIndex = EditorGUILayout.Popup("Type", selectedTypeIndex, types.ToArray());

        // Conditionally display the custom type input field
        if (types[selectedTypeIndex] == "Custom")
        {
            customTypeInput = EditorGUILayout.TextField("Custom Type", customTypeInput);
        }

        if (GUILayout.Button("Generate and Apply Description") && selectedGameObject != null)
        {
            GenerateAndApplyDescription(selectedGameObject);
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

    private string CenterTextWithinDashes(string text, int totalLength)
    {
        int padding = (totalLength - text.Length) / 2; // Calculate padding for each side
        string paddedText = new string('-', padding) + text + new string('-', totalLength - text.Length - padding);
        return paddedText;
    }


}
