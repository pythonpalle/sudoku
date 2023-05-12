using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;
    
    private TileBehaviour lastTileReference;

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
    }
    
    private void HandleMoveTileSelectWithKeys()
    {
        //if (!HasSelectedTiles) return;
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
                lastTileReference.HandleSelectPublic();
        }
    }
}
