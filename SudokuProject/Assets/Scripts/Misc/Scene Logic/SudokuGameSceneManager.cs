using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SudokuGameSceneManager : MonoBehaviour
{
    [SerializeField] private GeneratorPort generatorPort;
    
    private string startSceneName = "Scenes/Start Scene";
    private string puzzleSceneName = "Scenes/Puzzle Select Scene"; 
    private string gameSceneName = "Scenes/Game Scene";

    public void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void LoadPuzzleSelectScene()
    {
        SceneManager.LoadScene(puzzleSceneName);
    }
    
    public void LoadRandom()
    {
        generatorPort.GenerationType = GridGenerationType.random;
        SaveManager.SetGenerationType(generatorPort.GenerationType);
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void LoadCreateOwnScene()
    {
        generatorPort.GenerationType = GridGenerationType.empty;
        SceneManager.LoadScene(gameSceneName);
        //SceneManager.LoadScene(createOwnSceneName);
    }
}
