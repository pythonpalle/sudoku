using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour, IHasCommand
{
    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private bool pointerOverGrid = false;
    
    private TileBehaviour lastTileReference;
    
    private TileBehaviour lastTileClicked;
    private bool lastSelectionWasDoubleClick;
    private float timeOfLastClick;
    private float maxTimeForDoubleClick = 0.5f;

    private int stateCounter = 0;
    private List<List<TileBehaviour>> selectionHistory = new List<List<TileBehaviour>>();
    private List<TileBehaviour> lastSelectedInBeforeUndo;


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

        EventManager.OnNewCommand += OnNewCommand;
        EventManager.OnUndo += OnUndo;
        EventManager.OnRedo += OnRedo;
        
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
        
        EventManager.OnNewCommand -= OnNewCommand;
        EventManager.OnUndo -= OnUndo;
        EventManager.OnRedo -= OnRedo;

        EventManager.OnUIElementHover -= OnUIElementHover;
        EventManager.OnUIElementExit -= OnUIElementExit;
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

    public void SetPointerOverGrid(bool value)
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
        if (selectionObject.SelectionKeyDown && !pointerOverGrid)
        {
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

    public void OnNewCommand()
    {
        var selectedTiles = new List<TileBehaviour>(selectionObject.SelectedTiles);
        
        while (selectionHistory.Count > stateCounter)
        {
            selectionHistory.RemoveAt(selectionHistory.Count-1);
        }
        
        selectionHistory.Add(selectedTiles);
        stateCounter++;
    }

    private void ChangeState(bool undo)
    {
        int prevCounter = stateCounter;
        
        if (undo)
            stateCounter--; 
        else
            stateCounter++;
        
        // at the first undo, save current selection
        if (undo && stateCounter == selectionHistory.Count - 1)
        {
            lastSelectedInBeforeUndo = new List<TileBehaviour>(selectionObject.SelectedTiles);
        }
        // at last redo, select the saved selection
        else if (!undo && stateCounter == selectionHistory.Count + 1)
        {
            Debug.Log("Select last before undo!");
            if (lastSelectedInBeforeUndo != null)
            {
                DeselectAllTiles();
                foreach (var tile in lastSelectedInBeforeUndo)
                {
                    tile.Select();
                }
            }
        }
        
        // undo when has no states
        if (undo && stateCounter <= 0)
        {
            DeselectAllTiles();
            stateCounter = 1;
            return;
        }
        
        // redo at the end of the commands
        if (!undo && stateCounter > selectionHistory.Count)
        {
            stateCounter --;
            return;
        }
        
        // select the tiles that was affected by the last undo/redo
        DeselectAllTiles();
        int chosenCounter = undo ? prevCounter : stateCounter;
        var selectedTiles = selectionHistory[chosenCounter - 1];
        foreach (var tile in selectedTiles)
        {
            tile.Select();
        }
    }

    private void OnUndo()
    {
        ChangeState(true);
    }
    
    private void OnRedo()
    {
        ChangeState(false);
    }
}
