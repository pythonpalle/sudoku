using System;
using System.Collections;
using System.Collections.Generic;
using Command;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private GridPort gridPort;
    [SerializeField] private bool pointerOverGrid = false;
    
    private TileBehaviour lastTileReference;
    
    private TileBehaviour lastTileClicked;
    private bool lastSelectionWasDoubleClick;
    private float timeOfLastClick;
    private float maxTimeForDoubleClick = 0.5f;

    private List<TileBehaviour> lastSelectedInBeforeUndo;


    private void Start()
    {
        CommandManager.instance.OnCommandRedo += OnCommandRedo;
        CommandManager.instance.OnCommandUndo += OnCommandUndo;
        CommandManager.instance.OnUndoFail += OnUndoFail;
    }

    private void OnEnable()
    {
        EventManager.OnTileSelect += OnTileSelect;
        EventManager.OnTileDeselect += OnTileDeselect;
        
        EventManager.OnTilePointerDown += OnTilePointerDown;
        EventManager.OnTilePointerUp += OnTilePointerUp;
        EventManager.OnTilePointerEnter += OnTilePointerEnter;
        
        selectionObject.OnSendTileReference += OnSendTileReference;
        
        EventManager.OnUIElementHover += OnUIElementHover;
        EventManager.OnUIElementExit += OnUIElementExit;
        
        selectionObject.ClearSelectedTiles(); 
        selectionObject.SetSelectionMode(SelectionMode.None); 
    }
    
    private void OnDisable()
    {
        EventManager.OnTileSelect -= OnTileSelect;
        EventManager.OnTileDeselect -= OnTileDeselect; 
        
        EventManager.OnTilePointerDown -= OnTilePointerDown;
        EventManager.OnTilePointerUp -= OnTilePointerUp;
        EventManager.OnTilePointerEnter -= OnTilePointerEnter;
        
        selectionObject.OnSendTileReference -= OnSendTileReference;
        
        EventManager.OnUIElementHover -= OnUIElementHover;
        EventManager.OnUIElementExit -= OnUIElementExit;
        
        CommandManager.instance.OnCommandRedo -= OnCommandRedo;
        CommandManager.instance.OnCommandUndo -= OnCommandUndo;
        
        CommandManager.instance.OnUndoFail -= OnUndoFail;
    }

    private void OnUndoFail()
    {
        DeselectAllTiles();
    }

    private void OnCommandUndo(SudokuCommand command)
    {
        SelectFromCommand(command); 
    }

    private void OnCommandRedo(SudokuCommand command)
    {
        SelectFromCommand(command);
    }

    private void SelectFromCommand(SudokuCommand command)
    {
        if (command is EffectedTilesCommand effectedTilesCommand)
        {
            DeselectAllTiles();
            
            var tiles = gridPort.GetTilesFromInts(effectedTilesCommand.effectedIndexes);
            foreach (var tile in tiles)
            {
                tile.Select();
            }
        }
    }

    private void OnUIElementHover()
    {
        SetPointerOverGrid(true);
    }

    private void OnUIElementExit()
    {
        SetPointerOverGrid(false);
    }

    private void OnTileSelect(TileBehaviour tile)
    {
        selectionObject.TryAdd(tile);
    }

    private void OnTileDeselect(TileBehaviour tile)
    {
        selectionObject.TryRemove(tile);
    }
    
    private void OnSendTileReference(TileBehaviour tile)
    {
        lastTileReference = tile; 
    }

    private void SetPointerOverGrid(bool value)
    {
        pointerOverGrid = value;
    }
    
    private void OnTilePointerDown(TileBehaviour tile)
    {
        bool doubleClick = Time.time < timeOfLastClick + maxTimeForDoubleClick;
        if (doubleClick && tile == lastTileClicked)
        {
            EventManager.SelectAllTilesWithNumber(tile);
            lastSelectionWasDoubleClick = true;
        }
        else
        {
            if (selectionObject.multiSelectKeyIsPressed && tile.isSelected)
            {
                selectionObject.SetSelectionMode(SelectionMode.Deselecting);
                tile.Deselect();
            }
            else
            {
                if (!selectionObject.multiSelectKeyIsPressed)
                    selectionObject.DeselectAllTiles();
                
                selectionObject.SetSelectionMode(SelectionMode.Selecting); 
                tile.Select();
            }

            lastSelectionWasDoubleClick = false;
        }

        timeOfLastClick = Time.time;
        lastTileClicked = tile;
    }
    
    private void OnTilePointerUp(TileBehaviour tile)
    {
        if (tile.isSelected)
        {
            selectionObject.SetSelectionMode(SelectionMode.None);
        }
    }
    
    private void OnTilePointerEnter(TileBehaviour tile)
    {
        TilePointerEnter(tile);
    }
    
    private void TilePointerEnter(TileBehaviour tile)
    {
        HandleDragSelection(tile);
        EventManager.UIElementHover();
    }
    
    private bool HandleDragSelection(TileBehaviour tile)
    {
        if (!selectionObject.SelectionKeyDown)
            return false;
        
        if (selectionObject.IsSelecting)
        {
            tile.Select();
            return true;
        } 
        else if (selectionObject.IsDeselecting)
        {
            tile.Deselect();
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (!GameStateManager.gameIsActive)
            return;
        
        HandleMoveTileSelectWithKeys();
        HandleAllCellsSelectDetection();
        HandleRemoveSelection();
        HandleEnterButtonsDetection();
    }

    private void HandleAllCellsSelectDetection()
    {
        if (selectionObject.AllCellsKeyIsPressed)
        {
            EventManager.SelectAllTilesNumber();
        }
    }

    private void HandleRemoveSelection()
    {
        if (selectionObject.SelectionKeyPressed && !pointerOverGrid)
        {
           Debug.Log("Deselecte"); 
            
            DeselectAllTiles();
            selectionObject.SetSelectionMode(SelectionMode.Selecting);
        }
    }

    private void HandleEnterButtonsDetection()
    {
        if (selectionObject.colorSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.ColorMark);
            return;
        } 
        else if (selectionObject.cornerSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.CornerMark);
            return;
        } 
        else if (selectionObject.centerSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.CenterMark);
            return;
        }

        else if (selectionObject.colorSelectKeyIsReleased )
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
            return;
        } 
        
        else if (selectionObject.cornerSelectKeyIsReleased )
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
            return;
        } 
        
        else if (selectionObject.centerSelectKeyIsReleased )
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
            return;
        }
    }

    private bool CheckColorButtonDetection()
    {
        if (selectionObject.colorSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.ColorMark);
            return true;
        } 
        else if (selectionObject.colorSelectKeyIsReleased)
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
            return true;
        }

        return false;
    }

    private void HandleCornerButtonDetection()
    {
        if (selectionObject.cornerSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.CornerMark);
        } 
        else if (selectionObject.cornerSelectKeyIsReleased)
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
        }
    }
    
    private void HandleCenterButtonDetection()
    {
        if (selectionObject.centerSelectKeyIsPressed)
        {
            EventManager.SelectButtonClicked(EnterType.CenterMark);
        } 
        else if (selectionObject.centerSelectKeyIsReleased)
        {
            EventManager.SelectButtonClicked(EnterType.DigitMark);
        }
    }

    private void DeselectAllTiles()
    {
        selectionObject.DeselectAllTiles();
    }
    
    private void HandleMoveTileSelectWithKeys()
    {
        if (!selectionObject.HasSelectedTiles) return;

        // if the user just double clicked, the last tile is the one that was just clicked.
        // otherwise, it is the last tile selected by dragging
        TileBehaviour lastTile = lastSelectionWasDoubleClick 
            ? lastTileClicked 
            : selectionObject.LastSelected();

        int row = lastTile.row;
        int col = lastTile.col;

        bool moveKeyPressed = false;

        if (selectionObject.upKeyIsPressed)
        {
            row = (row + 8) % 9;
            moveKeyPressed = true;
        } 
        else if (selectionObject.downKeyIsPressed)
        {
            row = (row + 10) % 9;
            moveKeyPressed = true;
        }
        else if (selectionObject.leftKeyIsPressed)
        {
            col = (col + 8) % 9;
            moveKeyPressed = true;
        }
        else if (selectionObject.rightKeyIsPressed)
        {
            col = (col + 10) % 9;
            moveKeyPressed = true;
        }
        
        if (moveKeyPressed)
        {
            if (!selectionObject.multiSelectKeyIsPressed)
                DeselectAllTiles();

            selectionObject.SendTileRequest(row, col);
            
            if (lastTileReference)
                lastTileReference.Select();

            lastSelectionWasDoubleClick = false;
        }
    }
}
