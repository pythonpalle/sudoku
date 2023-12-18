using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleSelect;
using Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SudokuGameSceneManager : MonoBehaviour
{
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private PuzzleSelectPort selectPort;
    [SerializeField] private ScenePort scenePort;

    private static string startSceneName = "Start Scene";
    public static string puzzleSelectSceneName = "Puzzle Select Scene"; 
    private static string gameSceneName = "Game Scene";
    
    public void OnEnable()
    {
        selectPort.OnSelectAndLoad += OnSelectAndLoad;

        scenePort.OnCallLoadPuzzleSelectScene += LoadPuzzleSelectScene;
        scenePort.OnCallLoadRandom += LoadRandom;

        SaveManager.OnPuzzleSaveCreated += OnPuzzleSaveCreated;
        SaveManager.OnPuzzleReset += OnPuzzleReset;
    }
    
    public void OnDisable()
    {
        selectPort.OnSelectAndLoad -= OnSelectAndLoad;
        
        scenePort.OnCallLoadPuzzleSelectScene -= LoadPuzzleSelectScene;
        scenePort.OnCallLoadRandom -= LoadRandom;
        
        SaveManager.OnPuzzleSaveCreated -= OnPuzzleSaveCreated;
        SaveManager.OnPuzzleReset -= OnPuzzleReset;
    }

    private void OnPuzzleReset(PuzzleDataHolder data)
    {
        OnLoadPuzzle();
    }

    private void OnPuzzleSaveCreated()
    {
        if (generatorPort.GenerationType == GridGenerationType.empty)
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
        generatorPort.GenerationType = GridGenerationType.loaded;
        //SaveManager.SetGenerationType(generatorPort.GenerationType);
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
        generatorPort.GenerationType = GridGenerationType.random;
        //SaveManager.SetGenerationType(generatorPort.GenerationType);
        LoadGameScene();
    }
    
    public void LoadCreateOwnScene()
    {
        generatorPort.GenerationType = GridGenerationType.empty;
        LoadGameScene();
    }
}
