using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GridPort _gridPort;
    [SerializeField] private GeneratorPort _generatorPort;

    private string successfulSaveString = "Progress Saved!";
    private SaveRequestLocation location = SaveRequestLocation.SaveButton;
    
    private void OnEnable()
    {
        SaveManager.OnSuccessfulSave += OnSuccessfulSave;
    }
    
    private void OnDisable()
    {
        SaveManager.OnSuccessfulSave -= OnSuccessfulSave;
    }

    private void OnSuccessfulSave(SaveRequestLocation location)
    {
        // Don't display popup after a puzzle has just been created
        if (_generatorPort.GenerationType == GridGenerationType.empty)
        {
            return;
        }
        
        if (this.location == location)
            DisplaySavePopup();
    }

    private void DisplaySavePopup()
    {
        EventManager.DisplayFloatingPopupText(successfulSaveString, transform.position);
    }

    public void OnSaveButtonPressed()
    {
        if (SaveManager.TrySave(location))
        {
            DisplaySavePopup();
        }
        
        SaveGridString();
    }
    
    private void SaveGridString() 
    {
        _gridPort.RequestGrid();
        string gridString = _gridPort.GetGridAsSeed();
        GUIUtility.systemCopyBuffer = gridString;
        Debug.Log(gridString); 
    }
}
