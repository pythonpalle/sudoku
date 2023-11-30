using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GridPort _gridPort;

    public void SaveGrid()
    {
        _gridPort.RequestGrid();
        string gridString = _gridPort.GetGridAsSeed();
        
        GUIUtility.systemCopyBuffer = gridString;
        Debug.Log(gridString); 
    }
}
