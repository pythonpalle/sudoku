using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
    public static UnityAction<SudokuGrid9x9> OnGridGenerated;

    public static void GenerateGrid(SudokuGrid9x9 grid)
    {
        OnGridGenerated?.Invoke(grid);
    }
}
