using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GridPort _gridPort;
    [SerializeField] private GeneratorPort _generatorPort;
    [SerializeField] private SaveRequestPort _saveRequestPort;

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
        Debug.Log($"Save button OnSuccSave, location: {location}");
        
        // Don't display popup after a puzzle has just been created
        if (_generatorPort.GenerationType == GridGenerationType.empty)
        {
            return;
        }

        // Don't display popup when exiting
        if (location == SaveRequestLocation.ExitGameButton)
            return;
        
        DisplaySavePopup();
    }

    private void DisplaySavePopup()
    {
        Debug.Log("Display Save popup!");
        EventManager.DisplayFloatingPopupText(successfulSaveString, transform.position);
    }

    public void OnSaveButtonPressed()
    {
        Debug.Log("OnSaveButtonPress");
        _saveRequestPort.Location = location;
        
        if (SaveManager.TrySave(location, _generatorPort.GenerationType))
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
