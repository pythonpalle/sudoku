using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private List<SudokuGrid9x9> gridHistory;

    private TileBehaviour[,] tileBehaviours = new TileBehaviour[9,9];

    [SerializeField] private List<GridBoxBehaviour> boxes;
    
    public static GridBehaviour Instance { get; private set; }

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
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnTileIndexSet += OnTileIndexSet;
        EventManager.OnNumberEnter += OnNumberEnter;
        EventManager.OnRemoveEntry += OnRemoveEntry;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
        EventManager.OnNumberEnter -= OnNumberEnter; 
        EventManager.OnRemoveEntry -= OnRemoveEntry;
    }

    private void Start()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].Setup(i);
        }
    }

    private void OnGridGenerated(SudokuGrid9x9 generatedGrid)
    {
        grid = generatedGrid;
        gridHistory = new List<SudokuGrid9x9>
        {
            new SudokuGrid9x9(grid)
        };
        
        SetupTileNumbers();
    }

    private void SetupTileNumbers()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                tileBehaviours[row, col].SetStartNumber(grid[row, col].Number);
            }
        }
    }

    private void OnTileIndexSet(int row,  int col, TileBehaviour tileBehaviour)
    {
        tileBehaviours[row, col] = tileBehaviour;
    }
    
    private void OnNumberEnter(List<TileBehaviour> tiles, EnterType enterType, int number)
    {
        switch (enterType)
        {
            case EnterType.NormalNumber:
                HandleEnterNormalNumbers(tiles, number);
                break;
        }
    }
    
    private void OnRemoveEntry(List<TileBehaviour> tiles, EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.NormalNumber:
                HandleRemoveNormalNumbers(tiles);
                break;
        }
    }
    
    private void HandleEnterNormalNumbers(List<TileBehaviour> selectedTiles, int number)
    {
        SudokuGrid9x9 newGrid = new SudokuGrid9x9(grid);
        
        foreach (var tileBehaviour in selectedTiles)
        {
            if (tileBehaviour.Permanent) continue;

            EnterNormalNumber(newGrid, tileBehaviour, number);
        }
        
        HandleRemoveContradictions(newGrid);

        gridHistory.Add(newGrid);
        HandleAddContradictionsInList(newGrid, selectedTiles, number);
        grid = new SudokuGrid9x9(newGrid);
    }

    private void HandleRemoveContradictions(SudokuGrid9x9 newGrid)
    {
        List<TileBehaviour> tilesWithContradiction = new List<TileBehaviour>();
        foreach (var tile in tileBehaviours)
        {
            if (tile.Contradicted)
                tilesWithContradiction.Add(tile);
        }

        foreach (var tile in tilesWithContradiction)
        {
            if (!CheckForContradiction(tile, newGrid))
            {
                tile.RemoveContradiction();
            }
        }
    }

    private bool CheckForContradiction(TileBehaviour tile, SudokuGrid9x9 newGrid)
    {
        var effectedTiles = GetEffectedTiles(tile);
        int tileNumber = tile.number;

        foreach (var effectedTile in effectedTiles)
        {
            if (effectedTile.number == tileNumber)
                return true;
        }

        return false;
    }

    private void EnterNormalNumber(SudokuGrid9x9 grid, TileBehaviour tileBehaviour, int number)
    {
        if (!tileBehaviour.TryUpdateNumber(number))
            return;

        int row = tileBehaviour.row;
        int col = tileBehaviour.col;

        grid.SetNumberToIndex(row, col, number);
    }
    
    private void HandleAddContradictionsInList(SudokuGrid9x9 newGrid, List<TileBehaviour> selectedTiles, int number)
    {
        foreach (var tile in selectedTiles)
        {
            if (tile.Permanent) continue;
            
            List<TileBehaviour> effectedTiles = GetEffectedTiles(tile);
            bool someTileContradicted = false;

            foreach (var effected in effectedTiles)
            {
                if (HandleTileContradiction(effected, number))
                {
                    someTileContradicted = true;
                }
            }
            
            if (someTileContradicted)
            {
                tile.SetContradiction();
            }
        }
    }

    private List<TileBehaviour> GetEffectedTiles(TileBehaviour tile)
    {
        List<TileBehaviour> effectedTiles = new List<TileBehaviour>();

        int tileRow = tile.row;
        int tileCol = tile.col;
        
        // Tiles in same col
        for (int row = 0; row < 9; row++)
        {
            if (row == tileRow) continue;
            
            effectedTiles.Add(tileBehaviours[row, tileCol]);
        }
        
        // tiles in same row
        for (int col = 0; col < 9; col++)
        {
            if (col == tileCol) continue;
            
            effectedTiles.Add(tileBehaviours[tileRow, col]);
        }
        
        // tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                int boxRow = topLeftBoxRow + deltaRow;
                int boxCol = topLeftBoxCol + deltaCol;

                if (tileRow == boxRow && tileCol == boxCol) continue;
                
                effectedTiles.Add(tileBehaviours[boxRow, boxCol]);
            } 
        }

        return effectedTiles;
    }

    private bool HandleTileContradiction(TileBehaviour tile, int number)
    {
        if (tile.number == number)
        {
            tile.SetContradiction();
            return true;
        }

        return false;
    }
    
    private void HandleRemoveNormalNumbers(List<TileBehaviour> selectedTiles)
    {
        SudokuGrid9x9 newGrid = new SudokuGrid9x9(grid);
        foreach (var tile in selectedTiles)
        {
            EnterNormalNumber(newGrid, tile, 0);
        }
        
        HandleRemoveContradictions(newGrid);
        
        gridHistory.Add(newGrid);
        grid = new SudokuGrid9x9(newGrid);
    }

    public TileBehaviour GetTileAtIndex(int row, int col)
    {
        return tileBehaviours[row, col];
    }
}
