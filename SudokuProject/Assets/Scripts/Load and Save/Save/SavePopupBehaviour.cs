using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePopupBehaviour : MonoBehaviour
{
    [SerializeField] private PopupWindow popupWindow;
    [SerializeField] private SudokuGameSceneManager gameSceneManager;
    
    [Header("User input")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI placeHolderText;
    [SerializeField] private TextMeshProUGUI userEnterText;
    
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
        // Get Name from inputField, send data to SaveManager,
        // let Save Manager save

        string puzzleSaveName = placeHolderString;

        if (ValidEnteredName())
        {
            puzzleSaveName = userEnterText.text;
        }
        
        Debug.Log("Chosen name: " + puzzleSaveName);
    }

    private bool ValidEnteredName()
    {
        string enteredString = userEnterText.text;
        
        // only 1 character and is invisible empty char
        if (enteredString.Length == 1 && (int) enteredString[0] == 8203)
        {
            return false;
        }
        
        if (!string.IsNullOrEmpty(enteredString) && enteredString.Trim().Length > 0)
        {
            // String is not empty, doesn't contain only spaces
            return true;
        }
        else
        {
            // String is empty or contains only spaces or has zero-width space
            return false;
        }
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

        placeHolderText.text = placeHolderString;
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
