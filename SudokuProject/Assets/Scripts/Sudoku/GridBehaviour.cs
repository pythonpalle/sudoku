using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private List<SudokuGrid9x9> gridHistory;

    private TileBehaviour[,] tileBehaviours = new TileBehaviour[9,9];

    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private List<GridBoxBehaviour> boxes;

    private EnterType lastKnownEnterType = EnterType.DigitMark;

    private void Start()
    {
        SetupBoxes();
    }

    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnTileIndexSet += OnTileIndexSet;
        EventManager.OnNumberEnter += OnNumberEnter;
        EventManager.OnRemoveEntry += OnRemoveEntry;
        EventManager.OnSelectAllTilesWithNumber += OnSelectAllTilesWithNumber;

        selectionObject.OnRequestTile += OnRequestTile;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
        EventManager.OnNumberEnter -= OnNumberEnter; 
        EventManager.OnRemoveEntry -= OnRemoveEntry;
        EventManager.OnSelectAllTilesWithNumber -= OnSelectAllTilesWithNumber;
        
        selectionObject.OnRequestTile -= OnRequestTile;
    }
    
    private void SetupBoxes()
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
    
    private void OnTileIndexSet(int row,  int col, TileBehaviour tileBehaviour)
    {
        tileBehaviours[row, col] = tileBehaviour;
    }
    
    private void OnNumberEnter(List<TileBehaviour> tiles, EnterType enterType, int number)
    {
        lastKnownEnterType = enterType;
        
        HandleEnterNumberToSelectedTiles(tiles, number, enterType);
        
        switch (enterType)
        {
            case EnterType.DigitMark:
                HandleRemoveContradictions();
                HandleAddContradictionsInList(tiles, number);
                break;
        }
    }
    
    private void OnRemoveEntry(List<TileBehaviour> tiles, EnterType enterType)
    {
        lastKnownEnterType = enterType;

        switch (enterType)
        {
            case EnterType.DigitMark:
                HandleRemoveNormalNumbers(tiles);
                break;
        }
    }
    
    private void OnSelectAllTilesWithNumber(TileBehaviour doubleClickTile)
    {
        if (doubleClickTile.HasDigit)
        {
            SelectAllTilesWithDigit(doubleClickTile);
        }
        else
        {
            if (doubleClickTile.CornerMarks.Count > 0)
                SelectAllTilesWithCorner(doubleClickTile);
            
            if (doubleClickTile.centerMarks.Count > 0)
                SelectAllTilesWithCenter(doubleClickTile);
        }
    }

    private void SelectAllTilesWithDigit(TileBehaviour doubleClickTile)
    {
        int number = doubleClickTile.number;
        if (number == 0) return;
        
        foreach (var tile in tileBehaviours)
        {
            if (tile.number == number)
                tile.HandleSelect();
        }
    }
    
    private void SelectAllTilesWithCorner(TileBehaviour doubleClickTile)
    {
        bool hasCornerDigit = doubleClickTile.CornerMarks.Count > 0;
        if (!hasCornerDigit) return;

        foreach (var corner in doubleClickTile.CornerMarks)
        {
            foreach (var tile in tileBehaviours)
            {
                if (tile.CornerMarks.Contains(corner))
                    tile.HandleSelect();
            }
        }
    }
    
    private void SelectAllTilesWithCenter(TileBehaviour doubleClickTile)
    {
        bool hasCenterDigit = doubleClickTile.centerMarks.Count > 0;
        if (!hasCenterDigit) return;

        foreach (var center in doubleClickTile.centerMarks)
        {
            foreach (var tile in tileBehaviours)
            {
                if (tile.centerMarks.Contains(center))
                    tile.HandleSelect();
            }
        }
    }

    private void OnRequestTile(int row, int col)
    {
        selectionObject.SendTileReference(tileBehaviours[row,col]);
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

    private void HandleEnterNumberToSelectedTiles(List<TileBehaviour> selectedTiles, int number, EnterType enterType)
    {
        // if all selected tiles have the same number, remove the number (change to zero)
        //number = SetNumberBasedOnAllSameDigit(selectedTiles, number, enterType);
        bool allTilesHaveSameDigit = CheckIfAllTilesHaveNumber(selectedTiles, number, enterType);

        foreach (var tileBehaviour in selectedTiles)
        {
            if (tileBehaviour.Permanent) continue;

            EnterTileNumber(tileBehaviour, number, enterType, allTilesHaveSameDigit);
        }
    }

    private bool CheckIfAllTilesHaveNumber(List<TileBehaviour> selectedTiles, int number, EnterType enterType)
    {
        foreach (var tile in selectedTiles)
        {
            // skip given clues
            if (tile.Permanent) continue;
            
            // if not digit entry, skip tiles that already has digit
            if (enterType != EnterType.DigitMark && tile.HasDigit) continue;

            if (!tile.HasSameNumber(number, enterType))
                return false;
        }

        return true;
    }
    
    private void EnterTileNumber(TileBehaviour tileBehaviour, int number, EnterType enterType, bool sameNumber)
    {
        if (tileBehaviour.Permanent)
            return;

        tileBehaviour.TryUpdateNumber(number, enterType, sameNumber);

        if (enterType == EnterType.DigitMark)
        {
            AddNumberToGrid(tileBehaviour, number, sameNumber);
        }
    }

    private void AddNumberToGrid(TileBehaviour tileBehaviour, int number, bool remove)
    {
        int row = tileBehaviour.row;
        int col = tileBehaviour.col;

        if (remove)
            number = 0;

        grid.SetNumberToIndex(row, col, number);
    }

    private void EnterNormalNumber(TileBehaviour tileBehaviour, int number)
    {
        if (tileBehaviour.Permanent)
            return;

        int row = tileBehaviour.row;
        int col = tileBehaviour.col;

        grid.SetNumberToIndex(row, col, number);
    }

    private void HandleRemoveContradictions( )
    {
        List<TileBehaviour> tilesWithContradiction = new List<TileBehaviour>();
        foreach (var tile in tileBehaviours)
        {
            if (tile.Contradicted)
                tilesWithContradiction.Add(tile);
        }

        foreach (var tile in tilesWithContradiction)
        {
            if (!CheckForContradiction(tile))
            {
                tile.RemoveContradiction();
            }
        }
    }

    private bool CheckForContradiction(TileBehaviour tile)
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
    
    
    private void HandleAddContradictionsInList(List<TileBehaviour> selectedTiles, int number)
    {
        foreach (var tile in selectedTiles)
        {
            if (tile.Permanent || tile.number == 0) continue;
            
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
        foreach (var tile in selectedTiles)
        {
            EnterNormalNumber(tile, 0);
            tile.TryUpdateNumber(0, EnterType.DigitMark, true);
        }
        
        HandleRemoveContradictions();
    }

    
}
