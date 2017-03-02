using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// This is a class that will change the timestamp in 
/// the header files for my queries. This will allow me to always
/// have the most recent timestamp for my visualisation 
/// </summary>
public class WriteNewHeaders : MonoBehaviour {

    public int lineToEdit = 5;

    private bool isWriting;
    private string sourceFile;
    private string[] arrayParsed;

    public bool IsWritting { get { return isWriting; } }

    public string SourceFile { get { return sourceFile; } set { sourceFile = value; } }


    /// <summary>
    /// Set the file locations that we have 
    /// </summary>
  /*  void Start()
    {
        //currentWriter = this;
    }*/

    /// <summary>
    /// This is a coroutine that will write the specified thing line to the file,
    /// and return true when it is done
    /// </summary>
    /// <param name="newLineText">The line that you want to add</param>
    /// <param name="fileLocation">The File location that you are writing to</param>
    /// <param name="lineToEdit">The line number that you are editing</param>
    /// <returns></returns>
    public IEnumerator WriteHeaders(string newLineText)
    {
        if (sourceFile == null || newLineText == null)
        {
            Debug.Log("Something is null in the writer!");
            yield break;
        }

        if (isWriting)
        {
            // You are already writing, stop
            Debug.Log("Busy writing a file!!");
            yield break;            
        }

        isWriting = true;

        Debug.Log(newLineText);
        if(arrayParsed == null)
        {
            // Send the file to a string array, where each element is one line
            arrayParsed = File.ReadAllLines(sourceFile);
        }

        // set the line we want to edit to the new line of text
        arrayParsed[lineToEdit - 1] = "\"gt\":" + "\"" + newLineText + "\"";
        // Write that file to the specified location
        File.WriteAllLines(sourceFile, arrayParsed);
        
        isWriting = false;

        yield return null;
    }

}
