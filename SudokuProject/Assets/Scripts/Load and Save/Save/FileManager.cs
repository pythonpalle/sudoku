using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string fileName, string fileContents)
    {
        string fullFilePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"File path: {fullFilePath}");

        try
        {
            File.WriteAllText(fullFilePath, fileContents);
            Debug.Log("Successfully written to file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to write to {fullFilePath} with exception {e}"));
        }

        return false;
    }
    
    public static bool LoadFromFile(string fileName, out string result)
    {
        string fullFilePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            result = File.ReadAllText(fullFilePath);
            Debug.Log("Successfully loaded from file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to load from {fullFilePath} with exception {e}"));
            result = string.Empty;
        }

        return false;
    }
}
