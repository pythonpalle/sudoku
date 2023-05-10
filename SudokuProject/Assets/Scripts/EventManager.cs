using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
    public static UnityAction<SudokuGrid9x9> OnGridGenerated;
    
    public static UnityAction<int,int, TileBehaviour> OnTileIndexSet;
    
    public static UnityAction<TileBehaviour> OnTileSelect;
    public static UnityAction<TileBehaviour> OnTileDeselect;

    public static void GenerateGrid(SudokuGrid9x9 grid)
    {
        OnGridGenerated?.Invoke(grid);
    }

    public static void SetTileIndex(int row, int col, TileBehaviour tileBehaviour)
    {
        OnTileIndexSet?.Invoke(row, col, tileBehaviour);
    }

    public static void SelectTile(TileBehaviour tileBehaviour)
    {
        OnTileSelect?.Invoke(tileBehaviour);
    }
    
    public static void DeselectTile(TileBehaviour tileBehaviour)
    {
        OnTileDeselect?.Invoke(tileBehaviour);
    }
}
