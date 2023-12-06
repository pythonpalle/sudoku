using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GridPort _gridPort;

    private string successfulSaveString = "Saved to clipboard!";
    public void OnSaveButtonPressed()
    {
        SaveGrid(); 
    }
    
    private void SaveGrid() 
    {
        _gridPort.RequestGrid();
        string gridString = _gridPort.GetGridAsSeed();
        
        EventManager.DisplayFloatingPopupText(successfulSaveString, transform.position);
        
        GUIUtility.systemCopyBuffer = gridString;
        Debug.Log(gridString); 
    }
}
