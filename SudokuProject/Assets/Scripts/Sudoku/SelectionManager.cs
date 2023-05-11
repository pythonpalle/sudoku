using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;

    private bool rightKeyIsPressed => Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    private bool leftKeyIsPressed => Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    private bool upKeyIsPressed => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    private bool downKeyIsPressed => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

    private bool moveKeyIsPressed => rightKeyIsPressed || leftKeyIsPressed || upKeyIsPressed || downKeyIsPressed;
    private bool multiSelectKeyIsPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

    private bool onNumberPad;

    private TileBehaviour tileReference;

    private void OnEnable()
    {
        EventManager.OnTileSelect += OnTileSelect;
        EventManager.OnTileDeselect += OnTileDeselect;

        selectionObject.OnSendTileReference += OnSendTileReference;
        
        selectionObject.ClearSelectedTiles(); 
    }
    
    private void OnDisable()
    {
        EventManager.OnTileSelect -= OnTileSelect;
        EventManager.OnTileDeselect -= OnTileDeselect;
        
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

    private void Update()
    {
        HandleSetSelect();
        HandleMoveTileSelectWithKeys();
    }

    private void HandleSetSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!multiSelectKeyIsPressed && !onNumberPad) 
                DeselectAllTiles();
            
            selectionObject.SetSelect(true);
            //IsSelecting = true;
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            selectionObject.SetSelect(false);
            //IsSelecting = false;
        }
    }

    private void DeselectAllTiles()
    {
        for (int i = selectionObject.SelectedTiles.Count - 1; i >= 0; i--)
        {
            TileBehaviour tile = selectionObject.SelectedTiles[i];
            tile.Deselect();
            selectionObject.SelectedTiles.RemoveAt(i);
        }
    }
    
    private void HandleMoveTileSelectWithKeys()
    {
        //if (!HasSelectedTiles) return;
        if (!selectionObject.HasSelectedTiles) return;

        var lastTile = selectionObject.SelectedTiles[^1];        
        
        int row = lastTile.row;
        int col = lastTile.col;

        bool moveKeyPressed = false;

        if (upKeyIsPressed)
        {
            row = (row + 8) % 9;
            moveKeyPressed = true;
        } 
        else if (downKeyIsPressed)
        {
            row = (row + 10) % 9;
            moveKeyPressed = true;
        }
        else if (leftKeyIsPressed)
        {
            col = (col + 8) % 9;
            moveKeyPressed = true;
        }
        else if (rightKeyIsPressed)
        {
            col = (col + 10) % 9;
            moveKeyPressed = true;
        }
        
        if (moveKeyPressed)
        {
            if (!multiSelectKeyIsPressed)
                DeselectAllTiles();

            selectionObject.SendTileRequest(row, col);
            
            if (tileReference)
                tileReference.HandleSelect();
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
