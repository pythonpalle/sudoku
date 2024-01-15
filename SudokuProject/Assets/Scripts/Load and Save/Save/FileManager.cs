using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEngine;

public static class FileManager
{
    private static readonly string fileExtenstion = ".txt";
    
    private static string GetFullFilePathName(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName) + fileExtenstion;
    }
    
    public static bool WriteToFile(string fileName, string fileContents)
    {
        string fullFilePath = GetFullFilePathName(fileName);
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

    public static bool RemoveFile(string fileName)
    {
        string fullFilePath = GetFullFilePathName(fileName);
        try
        {
            File.Delete(fullFilePath);
            Debug.Log("Successfully deleted file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to delete file {fullFilePath} with exception {e}"));
            return false;
        }
    }

    public static bool FileExists(string fileName)
    {
        string fullFilePath = GetFullFilePathName(fileName);
        return (File.Exists(fullFilePath));
    }
    
    public static bool LoadFromFile(string fileName, out string result)
    {
        string fullFilePath = GetFullFilePathName(fileName);
        result = string.Empty;
        
        try
        {
            result = File.ReadAllText(fullFilePath);
            Debug.Log("Successfully loaded from file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to load from {fullFilePath} with exception {e}"));
        }

        return false;
    }
    
    public static bool WriteAllBytes(string fileName, byte[] bytes, bool compress)
    {
        string fullFilePath = GetFullFilePathName(fileName);
        Debug.Log($"File path: {fullFilePath}");

        try
        {
            if (compress)
            {
                WriteAllBytesCompressed(bytes, fullFilePath);
            }
            else
            {
                File.WriteAllBytes(fullFilePath, bytes);
            }
            
            Debug.Log("Successfully written to file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to write to {fullFilePath} with exception {e}"));
        }

        return false;
    }

    private static void WriteAllBytesCompressed(byte[] bytes, string fullFilePath)
    {
        using (FileStream fileStream = File.Create(fullFilePath))
        {
            using (GZipStream compressionStream =
                new GZipStream(fileStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                compressionStream.Write(bytes, 0, bytes.Length);
            }
        }
    }

    public static bool ReadAllBytes(string fileName, out byte[] bytes, bool compress)
    {
        string fullFilePath = GetFullFilePathName(fileName);
        
        try
        {
            if (compress)
            {
                using (FileStream fileStream = File.OpenRead(fullFilePath))
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }
                    bytes = decompressedStream.ToArray();
                }
            }
            else
            {
                bytes = File.ReadAllBytes(fullFilePath);
            }
            
            Debug.Log("Successfully loaded from file!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(($"Failed to load from {fullFilePath} with exception {e}"));
        }

        bytes = null;
        return false;
    }
}
