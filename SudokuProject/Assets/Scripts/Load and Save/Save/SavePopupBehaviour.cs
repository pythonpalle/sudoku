using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Saving
{
    public class SavePopupBehaviour : MonoBehaviour
{
    [SerializeField] private PopupWindow popupWindow;
    [SerializeField] private SudokuGameSceneManager gameSceneManager;
    [SerializeField] private ValidNameChecker _nameChecker;

    [Header("SO")]
    [SerializeField] private DifficultyObject _difficultyObject;
    [SerializeField] private GeneratorPort generatorPort;
    
    private string placeHolderString = String.Empty;
    private SaveRequestLocation _location;

    private void OnEnable()
    {
        SaveManager.OnRequestFirstSave += OnRequestFirstSave;
    }
    
    private void OnDisable()
    {
        SaveManager.OnRequestFirstSave -= OnRequestFirstSave;
    }

    private void OnRequestFirstSave(SaveRequestLocation location)
    {
        _location = location;
        
        if (placeHolderString == String.Empty)
            SetPlaceHolderText();
        
        popupWindow.PopUp();
    }
    
    public void OnYesButtonPressed()
    {
        string puzzleSaveName = _nameChecker.GetPuzzleSaveName();

        // TODO: avgör svårighetsgrad genom att låta gridsolver lösa. Om ingen lösning, sätt extreme (Impossible?)

        PuzzleDifficulty difficulty = GetDifficulty();
        SaveManager.TryCreateNewPuzzleSave(puzzleSaveName, difficulty, generatorPort.GenerationType);

        if (_location == SaveRequestLocation.SaveButton)
        {
            popupWindow.Close();
        }
        else
        {
            gameSceneManager.LoadPuzzleSelectScene();
        }
    }

    private PuzzleDifficulty GetDifficulty()
    {
        return _difficultyObject.Difficulty;
    }

    private void SetPlaceHolderText()
    {
        int puzzleCount = 0;
        
        switch (generatorPort.GenerationType)
        {
            case GridGenerationType.empty:
                puzzleCount = SaveManager.GetTotalPuzzleCount() + 1;
                placeHolderString = $"Puzzle {puzzleCount}";
                break;
            
            case GridGenerationType.random:
                puzzleCount = SaveManager.GetPuzzleCount(_difficultyObject.Difficulty) + 1;
                placeHolderString = $"{_difficultyObject.Difficulty} {puzzleCount}";
                break;
        } 
        
        _nameChecker.SetPlaceHolder(placeHolderString);
    }

    public void OnNoButtonPressed()
    {
        switch (_location)
        {
            case SaveRequestLocation.SaveButton:
                popupWindow.Close();
                break;
            
            case SaveRequestLocation.ExitGameButton:
                gameSceneManager.LoadPuzzleSelectScene();
                break;
        }
    }
}
}

