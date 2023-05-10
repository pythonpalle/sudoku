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
                HandleNormalNumberEntry(tiles, number);
                break;
        }
    }

    private void HandleNormalNumberEntry(List<TileBehaviour> tiles, int number)
    {
        foreach (var tileBehaviour in tiles)
        {
            tileBehaviour.TryUpdateNumber(number);
        }
    }
}
