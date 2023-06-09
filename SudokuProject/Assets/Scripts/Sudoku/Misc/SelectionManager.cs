using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    
    
    [SerializeField] private SelectionObject selectionObject;
    
    private TileBehaviour lastTileReference;

    [SerializeField] private bool pointerOverGrid = false;
    
    private float timeOfLastClick;
    private float maxTimeForDoubleClick = 0.5f;

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
        if (doubleClick)
        {
            EventManager.SelectAllTilesWithNumber(tile);
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
        }

        timeOfLastClick = Time.time;
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

        TileBehaviour lastTile = selectionObject.LastSelected();      
        
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
        }
    }
}
