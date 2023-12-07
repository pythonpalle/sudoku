using System.Collections;
using System.IO;
using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class SaveTestBehaviour : MonoBehaviour, IHasPuzzleData
{
    public int myScore;
    public string seed;

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     
        //     
        //     FileManager.WriteToFile("test.txt", jsonString);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     if (FileManager.LoadFromFile("test.txt", out string jsonString))
        //     {
        //         
        //     }
        //      
        // }
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder)
    {
        throw new System.NotImplementedException();
    }

    public void LoadFromSaveData(PuzzleDataHolder dataHolder)
    {
        throw new System.NotImplementedException();
    }
}
