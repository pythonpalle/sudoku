using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class SavePopupBehaviour : MonoBehaviour
{
    [SerializeField] private PopupWindow popupWindow;
    [SerializeField] private SudokuGameSceneManager gameSceneManager;

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
        Debug.Log("Popup Save Window!");
        popupWindow.PopUp();
    }
    
    public void OnYesButtonPressed()
    {
        // Get Name from inputField, send data to SaveManager,
        // let Save Manager save
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
