using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private List<SudokuGrid9x9> gridHistory;

    private TileBehaviour[,] tiles = new TileBehaviour[9,9];

    [SerializeField] private List<GridBoxBehaviour> boxes;
    
    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnTileIndexSet += OnTileIndexSet;
        EventManager.OnNumberEnter += OnNumberEnter;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
        EventManager.OnNumberEnter -= OnNumberEnter; 
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
                tiles[row, col].SetStartNumber(grid[row, col].Number);
            }
        }
    }

    private void OnTileIndexSet(int row,  int col, TileBehaviour tileBehaviour)
    {
        tiles[row, col] = tileBehaviour;
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

    private void HandleEnterNormalNumbers(List<TileBehaviour> tiles, int number)
    {
        SudokuGrid9x9 newGrid = new SudokuGrid9x9(grid);
        
        foreach (var tileBehaviour in tiles)
        {
            if (tileBehaviour.Permanent) continue;

            EnterNormalNumber(newGrid, tileBehaviour, number);
        }
        
        gridHistory.Add(newGrid);
        HandleContradictionsInList(newGrid, tiles, number);
    }

    private void EnterNormalNumber(SudokuGrid9x9 grid, TileBehaviour tileBehaviour, int number)
    {
        tileBehaviour.TryUpdateNumber(number);

        int row = tileBehaviour.row;
        int col = tileBehaviour.col;

        SudokuGrid9x9 nextGrid = new SudokuGrid9x9(grid);
        nextGrid.SetNumberToIndex(row, col, number);
    }
    
    private void HandleContradictionsInList(SudokuGrid9x9 newGrid, List<TileBehaviour> tileBehaviours, int number)
    {
        foreach (var tile in tileBehaviours)
        {
            HandleTileContradiction(newGrid, tile, number);
        }
    }

    private void HandleTileContradiction(SudokuGrid9x9 newGrid, TileBehaviour tile, int number)
    {
        int tileRow = tile.row;
        int tileCol = tile.col;

        bool tileContradicted = false;
        
        // Check contradiction in row
        for (int row = 0; row < 9; row++)
        {
            if (row == tileRow) continue;

            if (tiles[row, tileCol].number == number)
            {
                tileContradicted = true;
                tiles[row, tileCol].SetContradiction();
            }
        }
        
        // Check contradiction in col
        for (int col = 0; col < 9; col++)
        {
            if (col == tileCol) continue;

            if (tiles[tileRow, col].number == number)
            {
                tileContradicted = true;
                tiles[tileRow, col].SetContradiction();
            }
        }
        
        // Check Contradiction in box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                int boxRow = topLeftBoxRow + deltaRow;
                int boxCol = topLeftBoxCol + deltaCol;

                if (tileRow == boxRow && tileCol == boxCol) continue;
                
                var boxTile = tiles[boxRow, boxCol];

                if (boxTile.number == number)
                {
                    tileContradicted = true;
                    boxTile.SetContradiction();
                }
            } 
        }

        if (tileContradicted)
        {
            tile.SetContradiction();
        }
        
    }
}
