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
    public static UnityAction<int> OnSelectAllTilesWithNumber;
    
    public static UnityAction<List<TileBehaviour>, EnterType, int> OnNumberEnter;
    public static UnityAction<List<TileBehaviour>, EnterType> OnRemoveEntry;

    public static UnityAction OnUIElementHover;
    public static UnityAction OnUIElementExit;
    public static UnityAction<EnterType> OnSelectButtonClicked;

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

    public static void EnterNumber(List<TileBehaviour> tiles, EnterType enterType, int number)
    {
        OnNumberEnter?.Invoke(tiles, enterType, number);
    }

    public static void RemoveEntry(List<TileBehaviour> tiles, EnterType enterType)
    {
        OnRemoveEntry?.Invoke(tiles, enterType);
    }

    public static void UIElementHover()
    {
        OnUIElementHover?.Invoke();
    }

    public static void UIElementExit()
    {
        OnUIElementExit?.Invoke();
    }

    public static void SelectAllTilesWithNumber(int number)
    {
        OnSelectAllTilesWithNumber?.Invoke(number);
    }

    public static void SelectButtonClicked(EnterType type)
    {
        OnSelectButtonClicked?.Invoke(type);
    }
}
