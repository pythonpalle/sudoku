using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
    public static UnityAction<SudokuGrid9x9> OnGridGenerated;
    public static UnityAction OnSetupTiles;
    
    public static UnityAction<int,int, TileBehaviour> OnTileIndexSet;
    
    public static UnityAction<TileBehaviour> OnTileSelect;
    public static UnityAction<TileBehaviour> OnTileDeselect;
    
    public static UnityAction<TileBehaviour> OnTilePointerDown;
    public static UnityAction<TileBehaviour> OnTilePointerUp;
    public static UnityAction<TileBehaviour> OnTilePointerEnter;
    
    public static UnityAction<TileBehaviour> OnSelectAllTilesWithNumber;
    public static UnityAction OnSelectAllTiles;
    
    public static UnityAction<SudokuEntry> OnUserNumberEnter;
    public static UnityAction<SudokuEntry> OnUserRemoveEntry;
    
    public static UnityAction<SudokuCommand> OnExecuteCommand;

    public static UnityAction OnNewCommand;
    public static UnityAction<SudokuCommand> OnAddCommand;
    

    public static UnityAction OnUIElementHover;
    public static UnityAction OnUIElementExit;
    public static UnityAction<EnterType> OnSelectButtonClicked;
    
    public static UnityAction OnPuzzleComplete;

    public static UnityAction OnUndo;
    public static UnityAction OnRedo;
    
    public static UnityAction<SudokuCommand> OnUndoCommand;
    public static UnityAction<SudokuCommand> OnRedoCommand;

    public static UnityAction<SudokuGrid9x9> OnImportGrid;
    
    public static UnityAction<string, Vector3> OnDisplayHoverText;
    public static UnityAction OnCancelHoverText;
    public static UnityAction<string, Vector3> OnDisplayFloatingPopupText;


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

    public static void UserEnterNumber(SudokuEntry entry)
    {
        OnUserNumberEnter?.Invoke(entry);
        //OnNewCommand?.Invoke();
    }

    public static void UserRemoveEntry(SudokuEntry entry)
    {
        OnUserRemoveEntry?.Invoke(entry);
        // OnNewCommand?.Invoke();
    }

    public static void UIElementHover()
    {
        OnUIElementHover?.Invoke();
    }

    public static void UIElementExit()
    {
        OnUIElementExit?.Invoke();
    }

    public static void SelectAllTilesWithNumber(TileBehaviour tile)
    {
        OnSelectAllTilesWithNumber?.Invoke(tile);
    }
    
    public static void SelectAllTilesNumber()
    {
        OnSelectAllTiles?.Invoke();
    }

    public static void SelectButtonClicked(EnterType type)
    {
        OnSelectButtonClicked?.Invoke(type);
    }
    
    public static void TilePointerDown(TileBehaviour tile)
    {
        OnTilePointerDown?.Invoke(tile);
    }

    public static void TilePointerUp(TileBehaviour tile)
    {
        OnTilePointerUp?.Invoke(tile);
    }

    public static void TilePointerEnter(TileBehaviour tile)
    {
        OnTilePointerEnter?.Invoke(tile);
    }

    public static void PuzzleComplete()
    {
        OnPuzzleComplete?.Invoke();
    }

    public static void Undo()
    {
       // OnUndo?.Invoke();
    }
    
    public static void Redo()
    {
        //OnRedo?.Invoke();
    }
    
    public static void Undo(SudokuCommand command)
    {
        OnUndoCommand?.Invoke(command);
    }
    
    public static void Redo(SudokuCommand command)
    {
        OnRedoCommand?.Invoke(command);
    }

    public static void TilesSetup()
    {
        OnSetupTiles?.Invoke();
    }

    public static void ImportGrid(SudokuGrid9x9 grid)
    {
        OnImportGrid?.Invoke(grid);
    }
    
    public static void DisplayHoverText(string text, Vector3 position)
    {
        OnDisplayHoverText?.Invoke(text, position);
    }
    
    public static void CancelHoverText()
    {
        OnCancelHoverText?.Invoke();
    }

    public static void DisplayFloatingPopupText(string text, Vector3 position)
    {
        OnDisplayFloatingPopupText?.Invoke(text, position);
    }

    public static void ExecuteCommand(SudokuCommand command)
    {
        OnExecuteCommand?.Invoke(command);
    }

    public static void AddNewCommand(SudokuCommand command)
    {
        OnAddCommand?.Invoke(command);
    }
}
