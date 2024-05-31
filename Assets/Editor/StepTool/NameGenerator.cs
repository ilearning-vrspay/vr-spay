using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;

namespace StepCreationTool
{
    public static class NameGenerator
    {
        private static readonly List<string> types = new List<string>
        {
            "Dialogue", "Instruction", "Image", "Video", "On-Screen Instructions", "Program-Action", "User Action", "Custom"
        };

        private static StepData stepData = new StepData();

        



        private static void updateVariables(StepGUI stepToolGUI)
        {
            stepData.GroupName = stepToolGUI.GroupName;
            stepData.StepName = stepToolGUI.StepName;
            stepData.StepTypeIndex = stepToolGUI.SelectedTypeIndex;
            stepData.CustomTypeInput = stepToolGUI.CustomTypeInput;
        }


        
        
        public static string SetupFullName(StepGUI stepToolGUI, bool constructName = true)
        {
            // Updates variables in stepToolGUI. Presumably, this is a function that refreshes or sets various UI elements or data structures.
            updateVariables(stepToolGUI);
            
            // If 'constructName' boolean flag is false, return the name of the currently active GameObject in the editor.
            if (constructName == false)
            {
                return Selection.activeGameObject.name;
            }
            
            // Retrieve the currently selected GameObject in the Unity Editor.
            GameObject selectedGameObject = Selection.activeGameObject;
            
            // Determine the type string. 
            // If the selected type from 'stepData' is not "Custom", use it; otherwise, use the user-input custom type, removing all spaces.
            string type = (types[stepData.StepTypeIndex] != "Custom" ? types[stepData.StepTypeIndex] : stepData.CustomTypeInput).Replace(" ", "");
            
            // Construct base name using group name, step name, and type for convention consistency.
            string baseName = $"{stepData.GroupName}_{stepData.StepName}_{type}";
            
            // Check if there is a selected GameObject and if it does not have an InstructionDeliveryController component,
            // if so, use it as the parent object; otherwise, set parent object to null.
            GameObject parentObject = selectedGameObject != null && selectedGameObject.GetComponent<InstructionDeliveryController>() == null ? selectedGameObject : null;
            
            // Determines prefix based on the selected and parent GameObjects, through some logic encapsulated in the 'DetermineStepPrefix' method.
            string prefix = DetermineStepPrefix(selectedGameObject, parentObject);
            
            // Concatenate the obtained prefix with the base name to form the full name of the step.
            string fullName = $"{prefix}_{baseName}";
            
            // Return the constructed full name which likely will be used as the identifier for the current step.
            return fullName;
        }

        private static string DetermineStepPrefix(GameObject selectedGameObject, GameObject parentObject = null)
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

                return DetermineNextRootStepName(parentObject);
            }
        }

        private static string DetermineChildStepName(GameObject parent)
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

        private static string DetermineNextRootStepName(GameObject parent = null)
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

        private static int ExtractLeadingNumber(string name)
        {
            var match = Regex.Match(name, @"^(\d+)_");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return 0; // No leading number found
        }

    }
}
