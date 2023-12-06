using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class SaveTestBehaviour : MonoBehaviour, ISavable
{
    public int myScore;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PopulateSaveData();

            var data = new SaveData();
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
            
            //LoadFromSaveData();
        }
    }

    public void PopulateSaveData(SaveData data)
    {
        data.score = myScore;
        string jsonString = data.ToJson();
            
        FileManager.WriteToFile("test.txt", jsonString);
    }

    public void LoadFromSaveData(SaveData data)
    {
        if (FileManager.LoadFromFile("test.txt", out string jsonString))
        {
            SaveData saveData = new SaveData();
            saveData.LoadFromJson(jsonString);
            myScore = saveData.score;
        }
    }
}
