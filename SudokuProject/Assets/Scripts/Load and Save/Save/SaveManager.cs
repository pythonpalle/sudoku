using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
   
}

[System.Serializable]
public class SaveData
{
    public bool testBool;
    public float[] testFloatArray;
    public int score;

    /// <summary>
    /// Converts the public fields of this game object to a JSON representation and returns it as a string.
    /// </summary>
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// Overrides the public fields in this object from the JSON string representation.
    /// </summary>
    public void LoadFromJson(string JsonString)
    {
        JsonUtility.FromJsonOverwrite(JsonString, this);
    }
}
