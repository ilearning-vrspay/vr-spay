using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xceed.Words.NET;
using System.IO;



public class DocxTest : MonoBehaviour
{
    public string docxFilePath;

    // Start is called before the first frame update
    void Start()
    {
        ReadDocxFile(docxFilePath);
    }

    private void ReadDocxFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is empty");
            return;
        }

        using (DocX doc = DocX.Load(filePath))
        {
            foreach (var paragraph in doc.Paragraphs)
            {
                Debug.Log(paragraph.Text);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
