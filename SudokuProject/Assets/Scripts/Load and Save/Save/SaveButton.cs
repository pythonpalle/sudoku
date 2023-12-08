using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GridPort _gridPort;

    private string successfulSaveString = "Saved to clipboard!";
    private SaveRequestLocation location = SaveRequestLocation.SaveButton;
    
    public void OnSaveButtonPressed()
    {
        if (SaveManager.TrySave(location))
        {
        }
        
        Debug.Log("Save button is pressed!");
        
        SaveGridString();
    }
    
    private void SaveGridString() 
    {
        _gridPort.RequestGrid();
        string gridString = _gridPort.GetGridAsSeed();
        
        EventManager.DisplayFloatingPopupText(successfulSaveString, transform.position);
        
        GUIUtility.systemCopyBuffer = gridString;
        Debug.Log(gridString); 
    }
}
