using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void OnQuitButtonPressed()
    {
        Debug.Log("Exit game in build!");
        Application.Quit();
    }
}
