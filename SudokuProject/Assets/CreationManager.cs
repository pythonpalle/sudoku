using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationManager : MonoBehaviour
{
    private CreationObject creationObject;

    private SudokuGrid9x9 grid;
    private void OnEnable()
    {
        creationObject.OnSendGridCopy += OnSendGridCopy;
    }
    
    private void OnDisable() 
    {
        creationObject.OnSendGridCopy -= OnSendGridCopy;
    }

    public void TryCreatePuzzle()
    {
        
    }

    private void OnSendGridCopy(SudokuGrid9x9 grid)
    {
        this.grid = grid;
    }
}
