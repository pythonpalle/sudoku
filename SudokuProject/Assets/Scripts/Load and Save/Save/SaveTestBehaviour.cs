using System.Collections;
using System.IO;
using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class SaveTestBehaviour : MonoBehaviour, ISavable
{
    public int myScore;
    public string seed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PopulateSaveData();

            var data = new SaveData
            {
                score = myScore,
                sudokuGames = new SudokuGameData[1]
            };
            data.sudokuGames[0] = new SudokuGameData(seed);
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
                seed = "";

                foreach (var number in saveData.sudokuGames[0].numbers)
                {
                    seed += number;
                }
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
