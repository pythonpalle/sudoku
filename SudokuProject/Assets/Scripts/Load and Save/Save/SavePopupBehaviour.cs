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
    [SerializeField] private ValidNameChecker _nameChecker;
    [SerializeField] private PuzzleSaveInfo puzzleSaveInfo;

    [Header("SO")]
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private ScenePort scenePort;
    
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

        PuzzleDifficulty difficulty = puzzleSaveInfo.GetDifficulty();
        SaveManager.TryCreateNewPuzzleSave(puzzleSaveName, _location, difficulty, generatorPort.GenerationType);

        if (_location == SaveRequestLocation.SaveButton)
        {
            popupWindow.Close();
        }
        else
        {
            scenePort.OnCallLoadPuzzleSelectScene();
        }
    }

    private void SetPlaceHolderText()
    {
        placeHolderString = puzzleSaveInfo.GetNameSuggestion();
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
                scenePort.CallLoadPuzzleSelectScene();
                break;
        }
    }
}
}

