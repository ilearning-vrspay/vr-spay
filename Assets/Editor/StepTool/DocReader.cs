using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xceed.Words.NET; // Import DocX library
using System.Text.RegularExpressions;



namespace StepCreationTool
{
    public static class DocReader
    {
        // Start is called before the first frame update
        public static List<StepGroupData> ReadDocxFile(string path)
        {
            List<StepGroupData> stepCommandList = new List<StepGroupData>(); 

            int stepsBuilt = 0;
            int retryCount = 3;
            int retryDelay = 1000;
            //list of stepData objects


            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    using (DocX doc = DocX.Load(path))
                    {
                        int stepNumber = 1;

                        // **StepGroup**
                        string stepGroupName = "";
                        List<string> numberedListParagraphs = ExtractNumberedListParagraphs(path);
                        foreach (var para in numberedListParagraphs)
                        {
                            Debug.Log(para);
                        }
                        foreach (var paragraph in doc.Paragraphs)
                        {
                            // {StepName}
                            // [StepType]
                            // <StepContents>
                            // `StepScript`

                            
                            bool stepGroupLine = paragraph.Text.Contains("**");
                            stepGroupName = paragraph.Text.Contains("**") ? ExtractBetween(paragraph.Text, "**", "**") : stepGroupName;

                            if (paragraph.Text != ""){
                                
                                if (stepGroupLine)
                                {
                                    StepGroupData stepGroupData = new StepGroupData
                                    {
                                        groupName = stepGroupName,
                                        stepList = new List<StepData>()
                                    };
                                    stepCommandList.Add(stepGroupData);
                                

                                } else {

                                    string stepName = paragraph.Text.Contains("{") ? ExtractBetween(paragraph.Text, "{", "}") : null;
                                    string stepType = paragraph.Text.Contains("[") ? ExtractBetween(paragraph.Text, "[", "]") : null;
                                    string stepContents = paragraph.Text.Contains("<") ? ExtractBetween(paragraph.Text, "<", ">") : null;
                                    string stepScript = paragraph.Text.Contains("`") ? ExtractBetween(paragraph.Text, "`", "`") : null;

                                    if (stepType == "User Action"){
                                        int userStepCount = stepCommandList[stepCommandList.Count - 1].stepList[stepCommandList[stepCommandList.Count - 1].stepList.Count - 1].userSteps.Count;
                                        var userStepChar = ((char)('a' + userStepCount)).ToString();
                                        var userStep = (stepNumber-1).ToString() + userStepChar;
                                        
                                        UserStepData userStepData = new UserStepData
                                        {
                                            SequenceNumber = userStep,
                                            GroupName = stepGroupName,
                                            StepName = stepName,    
                                            StepType = stepType,
                                            CustomTypeInput = stepContents,
                                            ScriptContent = stepScript,
                                            Description = "Description goes here"
                                        };


                                        stepCommandList[stepCommandList.Count - 1].stepList[stepCommandList[stepCommandList.Count - 1].stepList.Count - 1].userSteps.Add(userStepData);
                                    
                                    } else {

                                        StepData stepData = new StepData
                                        {
                                            SequenceNumber = stepNumber,
                                            GroupName = stepGroupName,
                                            StepName = stepName,
                                            StepType = stepType,
                                            CustomTypeInput = null,
                                            ScriptContent = stepScript,
                                            Description = "Description goes here"
                                        };
                                        stepCommandList[stepCommandList.Count - 1].addStep(stepData);
                                        stepNumber++;
                                        
                                    }
                                
                                }
                            }
                        
                        }
                    }
                    break;
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    System.Threading.Thread.Sleep(retryDelay);
                }
            }
            // for (int i = 0; i < stepCommandList.Count; i++)
            // {   
                
            //     Debug.Log("Step Group: " + stepCommandList[i].groupName);
            //     Debug.Log("///////------------///////");

            //     for (int j = 0; j < stepCommandList[i].stepList.Count; j++)
            //     {
            //         Debug.Log("---------------------------------------");
            //         Debug.Log("Step Name: " + stepCommandList[i].stepList[j].StepName);
            //         Debug.Log("Step Type: " + stepCommandList[i].stepList[j].StepType);
            //         Debug.Log("Step Contents: " + stepCommandList[i].stepList[j].CustomTypeInput);
            //         Debug.Log("Step Script: " + stepCommandList[i].stepList[j].ScriptContent);
            //     }
            // }

            Debug.Log("Step Command List Count: " + stepCommandList.Count);
            int totalSteps = 0;
            for (int i = 0; i < stepCommandList.Count; i++)
            {
                totalSteps += stepCommandList[i].stepList.Count;
            }
            Debug.Log("Total Steps: " + totalSteps);
            
            return stepCommandList;
        }

        private static string ExtractBetween(string text, string start, string end)
        {
            var escapedStart = Regex.Escape(start);
            var escapedEnd = Regex.Escape(end);

            var match = Regex.Match(text, $@"{escapedStart}(.*?){escapedEnd}");
            // Debug.Log($"Extracting between {start} and {end}: Found '{match.Groups[1].Value}' in '{text}'");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        static List<string> ExtractNumberedListParagraphs(string filePath)
        {
            List<string> numberedListParagraphs = new List<string>();

            using (DocX document = DocX.Load(filePath))
            {
                foreach (var paragraph in document.Paragraphs)
                {
                    if (IsNumberedList(paragraph))
                    {
                        numberedListParagraphs.Add(paragraph.Text);
                    }
                }
            }

            return numberedListParagraphs;
        }

        static bool IsNumberedList(Xceed.Document.NET.Paragraph paragraph)
        {
            // Check if the paragraph is part of a numbered list
            // The approach might vary based on how the numbering is set up
            return paragraph.IsListItem && paragraph.ListItemType == Xceed.Document.NET.ListItemType.Numbered;
        }

    
    }

    
}

