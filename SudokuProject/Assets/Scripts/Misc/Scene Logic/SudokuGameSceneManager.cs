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
    
    private string startSceneName = "Scenes/Start Scene";
    private string puzzleSceneName = "Scenes/Puzzle Select Scene"; 
    private string gameSceneName = "Scenes/Game Scene"; 

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
            Debug.Log("Load created puzzle!");
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
        SaveManager.SetGenerationType(generatorPort.GenerationType);
        LoadPuzzleSelectScene();
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

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
        LoadGameScene();
    }
    
    public void LoadCreateOwnScene()
    {
        generatorPort.GenerationType = GridGenerationType.empty;
        LoadGameScene();
    }
}
