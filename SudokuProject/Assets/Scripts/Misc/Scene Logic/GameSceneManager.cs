using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static string startSceneName = "Scenes/Start Scene";
    private static string puzzleSceneName = "Scenes/Puzzle Select Scene";
    private static string gameSceneName = "Scenes/Game Scene";
    private static string createOwnSceneName = "Scenes/Create Own Scene";

    public static void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }
    
    public static void LoadPuzzleSelectScene()
    {
        SceneManager.LoadScene(puzzleSceneName);
    }
    
    public static void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    
    public static void LoadCreateOwnScene()
    {
        SceneManager.LoadScene(createOwnSceneName);
    }
}
