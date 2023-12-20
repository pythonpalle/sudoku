using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void Awake()
    {
        SaveManager.TrySetCurrentUserData(out _);
    }


    public void OnQuitButtonPressed()
    {
        Debug.Log("Exit game in build!");
        Application.Quit();
    }
}
