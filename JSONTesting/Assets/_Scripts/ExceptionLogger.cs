using UnityEngine;
using System.Collections;
using System.IO;

public class ExceptionLogger : MonoBehaviour
{
    public string saveFile = @"Log.txt";
    private StringWriter logWriter;

    void OnEnable()
    {
        Application.RegisterLogCallback(ExceptionWriter);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    //This is an instance of a delegate, we'll cover these in more detail in the Advanced scripting chapter
    //Changing the types or order of the types in this method will break the code.
    //You can name it whatever you like.
    void ExceptionWriter(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Exception:
            case LogType.Error:
                using (StreamWriter writer = new StreamWriter(new FileStream(saveFile, FileMode.Append)))
                {
                    writer.WriteLine(type);
                    writer.WriteLine(logString);
                    writer.WriteLine(stackTrace);
                }
                break;
            default:
                break;
        }
    }
}
