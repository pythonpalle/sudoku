using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleSelect;
using Saving;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SudokuGameSceneManager : MonoBehaviour
{
    public static SudokuGameSceneManager instance;
    
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private PuzzleSelectPort selectPort;
    [SerializeField] private ScenePort scenePort;

    private static string startSceneName = "Start Scene";
    private static string puzzleSelectSceneName = "Puzzle Select Scene"; 
    private static string gameSceneName = "Game Scene";
    
    public static string PuzzleSelectSceneName => puzzleSelectSceneName;
    public static string GameSceneName => gameSceneName;
    
    private HashSet<string> visitedScenes = new HashSet<string>();
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        visitedScenes.Add(SceneManager.GetActiveScene().name);
    }
    
    public void OnEnable()
    {
        selectPort.OnSelectAndLoad += OnSelectAndLoad;

        scenePort.OnCallLoadPuzzleSelectScene += LoadPuzzleSelectScene;
        scenePort.OnCallLoadRandom += LoadRandom;

        SaveManager.OnPuzzleSaveCreated += OnPuzzleSaveCreated;
        SaveManager.OnPuzzleReset += OnPuzzleReset;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    public void OnDisable()
    {
        selectPort.OnSelectAndLoad -= OnSelectAndLoad;
        
        scenePort.OnCallLoadPuzzleSelectScene -= LoadPuzzleSelectScene;
        scenePort.OnCallLoadRandom -= LoadRandom;
        
        SaveManager.OnPuzzleSaveCreated -= OnPuzzleSaveCreated;
        SaveManager.OnPuzzleReset -= OnPuzzleReset;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool OnlyVisitedGameScene()
    {
        return visitedScenes.Count == 1 && visitedScenes.Contains(gameSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        string name = scene.name;
        visitedScenes.Add(name);
    }

    private void OnPuzzleReset(PuzzleDataHolder data)
    {
        OnLoadPuzzle();
    }

    private void OnPuzzleSaveCreated()
    {
        if (generatorPort.GetGenerationType() == GridGenerationType.empty)
        {
            OnLoadPuzzle();
        }
    }
    
    private void OnSelectAndLoad(PuzzleDataHolder puzzle)
    {
        SaveManager.SetCurrentPuzzle(puzzle);
        OnLoadPuzzle();
    }

    private void OnLoadPuzzle()
    {
        generatorPort.SetGenerationType(GridGenerationType.loaded);
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        LoadScene(gameSceneName);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadStartScene()
    {
        LoadScene(startSceneName);
    }

    public void LoadPuzzleSelectScene()
    {
        LoadScene(puzzleSelectSceneName);
    }
    
    public void LoadRandom()
    {
        generatorPort.SetGenerationType(GridGenerationType.random);
        LoadGameScene();
    }
    
    public void LoadCreateOwnScene()
    {
        generatorPort.SetGenerationType(GridGenerationType.empty);
        LoadGameScene();
    }
}
