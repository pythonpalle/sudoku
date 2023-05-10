using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public List<TileBehaviour> SelectedTiles;// { get; private set; } = new List<TileBehaviour>();
    
    public static SelectionManager Instance { get; private set; }
    public bool HasSelectedTiles => SelectedTiles.Count > 0;

    public bool IsSelecting;// { get; private set; }

    private bool rightKeyIsPressed => Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    private bool leftKeyIsPressed => Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    private bool upKeyIsPressed => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    private bool downKeyIsPressed => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

    private bool moveKeyIsPressed => rightKeyIsPressed || leftKeyIsPressed || upKeyIsPressed || downKeyIsPressed;
    private bool multiSelectKeyIsPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

    private void Awake()
    {
        MakeSingleton();
    }
    
    private void MakeSingleton()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnEnable()
    {
        EventManager.OnTileSelect += OnTileSelect;
        EventManager.OnTileDeselect += OnTileDeselect;
    }
    
    private void OnDisable()
    {
        EventManager.OnTileSelect -= OnTileSelect;
        EventManager.OnTileDeselect -= OnTileDeselect;
    }

    private void OnTileSelect(TileBehaviour tile)
    {
        if (!SelectedTiles.Contains(tile))
        {
            SelectedTiles.Add(tile);
        }
    }

    private void OnTileDeselect(TileBehaviour tile)
    {
        if (SelectedTiles.Contains(tile))
        {
            SelectedTiles.Remove(tile);
        }
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
            if (!multiSelectKeyIsPressed)
                DeselectAllTiles();
            
            IsSelecting = true;
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            IsSelecting = false;
        }
    }

    private void DeselectAllTiles()
    {
        for (int i = SelectedTiles.Count - 1; i >= 0; i--)
        {
            TileBehaviour tile = SelectedTiles[i];
            tile.Deselect();
            SelectedTiles.RemoveAt(i);
        }
    }
    
    private void HandleMoveTileSelectWithKeys()
    {
        if (!HasSelectedTiles) return;

        var lastTile = SelectedTiles[^1];        
        
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
            
            TileBehaviour selectTile = GridBehaviour.Instance.GetTileAtIndex(row, col);
            selectTile.HandleSelect();
        }
    }
}
