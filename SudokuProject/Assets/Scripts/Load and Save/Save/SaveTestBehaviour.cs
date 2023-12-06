using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class SaveTestBehaviour : MonoBehaviour
{
    public int myScore;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData data = new SaveData();
            data.score = myScore;
            string jsonString = data.ToJson();
            
            FileManager.WriteToFile("test.txt", jsonString);
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (FileManager.LoadFromFile("test.txt", out string jsonString))
            {
                SaveData saveData = new SaveData();
                saveData.LoadFromJson(jsonString);
                myScore = saveData.score;
            }

        }
    }
}
