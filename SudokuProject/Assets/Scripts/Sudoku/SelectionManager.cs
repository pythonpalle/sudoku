using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;


    private bool onNumberPad;

    private TileBehaviour tileReference;
    

    private void OnEnable()
    {
        EventManager.OnTileSelect += OnTileSelect;
        EventManager.OnTileDeselect += OnTileDeselect;
        EventManager.OnSetSelectionMode += OnSetSelection;

        selectionObject.OnSendTileReference += OnSendTileReference;
        
        selectionObject.ClearSelectedTiles(); 
        selectionObject.SetSelectionMode(SelectionMode.None);
    }
    
    private void OnDisable()
    {
        EventManager.OnTileSelect -= OnTileSelect;
        EventManager.OnTileDeselect -= OnTileDeselect;
        EventManager.OnSetSelectionMode -= OnSetSelection;

        selectionObject.OnSendTileReference -= OnSendTileReference;
    }

    private void OnTileSelect(TileBehaviour tile)
    {
        if (!selectionObject.SelectedTiles.Contains(tile))
        {
            selectionObject.SelectedTiles.Add(tile);
        }
    }

    private void OnTileDeselect(TileBehaviour tile)
    {
        if (selectionObject.SelectedTiles.Contains(tile))
        {
            selectionObject.SelectedTiles.Remove(tile);
        }
    }
    
    private void OnSendTileReference(TileBehaviour tile)
    {
        tileReference = tile;
    }
    
    private void OnSetSelection(SelectionMode mode)
    {
        selectionObject.SetSelectionMode(mode);
    }

    private void Update()
    {
        HandleEnterButtonsDetection();
        HandleMoveTileSelectWithKeys();
    }

    private void HandleEnterButtonsDetection()
    {
        HandleCornerButtonDetection();
        HandleCenterButtonDetection();
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
        
        // for (int i = selectionObject.SelectedTiles.Count - 1; i >= 0; i--)
        // {
        //     TileBehaviour tile = selectionObject.SelectedTiles[i];
        //     tile.Deselect();
        //     selectionObject.SelectedTiles.RemoveAt(i);
        // }
    }
    
    private void HandleMoveTileSelectWithKeys()
    {
        //if (!HasSelectedTiles) return;
        if (!selectionObject.HasSelectedTiles) return;

        var lastTile = selectionObject.SelectedTiles[^1];        
        
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
            
            if (tileReference)
                tileReference.HandleSelectPublic();
        }
    }
    
    public void OnNumberKeyHover()
    {
        onNumberPad = true;
    }
    
    public void OnNumberKeyExit()
    {
        onNumberPad = false;
    }
}
