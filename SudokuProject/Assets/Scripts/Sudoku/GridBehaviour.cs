using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
    }

    private void OnGridGenerated(SudokuGrid9x9 arg0)
    {
        throw new NotImplementedException();
    }
}
