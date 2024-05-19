using UnityEditor;
using UnityEngine;
using Xceed.Words.NET; // Import DocX library
using System.IO;
using System;
using System.Text.RegularExpressions;

public class DocxToolWindow : EditorWindow
{
    private string docxFilePath = ""; // Path to the .docx file
    private string groupName = ""; // Current group name

    [MenuItem("Tools/Docx Tool")]
    public static void ShowWindow()
    {
        GetWindow<DocxToolWindow>("Docx Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Docx Reader Tool", EditorStyles.boldLabel);

        // Field for the docx file path
        docxFilePath = EditorGUILayout.TextField("Docx File Path", docxFilePath);

        if (GUILayout.Button("Read Docx"))
        {
            ReadDocxFile(docxFilePath);
        }
    }

    private void ReadDocxFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("Invalid file path.");
            return;
        }

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
                                groupName = text.Substring(2, text.Length - 4).Trim();
                                Debug.Log($"Group Name: {groupName}");
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
                            }
                            else if (stepType == "user action")
                            {
                                // Treat as a user action if tagged with [user action]
                                Debug.Log($"User Action: {text}");
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
}
