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
    
    private void Start()
    {
        SetupBoxes();
    }

    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnTileIndexSet += OnTileIndexSet;
        EventManager.OnNumberEnter += OnNumberEnter;
        EventManager.OnRemoveEntry += OnRemoveEntryEvent;
        EventManager.OnSelectAllTilesWithNumber += OnSelectAllTilesWithNumber;
        EventManager.OnSelectAllTiles += OnSelectAllTiles;

        selectionObject.OnRequestTile += OnRequestTile;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
        EventManager.OnNumberEnter -= OnNumberEnter; 
        EventManager.OnRemoveEntry -= OnRemoveEntryEvent;
        EventManager.OnSelectAllTilesWithNumber -= OnSelectAllTilesWithNumber;
        EventManager.OnSelectAllTiles -= OnSelectAllTiles;

        selectionObject.OnRequestTile -= OnRequestTile;
    }

    private void OnSelectAllTiles()
    {
        SelectAllTiles();
    }

    private void SelectAllTiles()
    {
        foreach (var tile in tileBehaviours)
        {
            if (!tile.isSelected)
            {
                tile.Select();
            }
        }    
    }

    private bool SkipTile(TileBehaviour tile, EnterType enterType)
    {
        return tile.Permanent && enterType != EnterType.ColorMark;
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
        HandleEnterNumberToSelectedTiles(tiles, number, enterType);
        
        switch (enterType)
        {
            case EnterType.DigitMark:
                HandleRemoveContradictions();
                HandleAddContradictionsInList(tiles, number);
                HandleCompletion();
                break;
        }
    }
    
    private void HandleCompletion()
    {
        bool complete = CheckComplete();
        if (complete)
        {
            Debug.Log("You solved it, hurray!");
        }
    }

    private bool CheckComplete()
    {
        foreach (var tile in tileBehaviours)
        {
            if (!tile.HasDigit || tile.Contradicted)
                return false;
        }

        return true;
    }

    private void OnRemoveEntryEvent(List<TileBehaviour> tiles, EnterType enterType, bool colorRemoval)
    {
        // special case for color removal, since it can't remove anything else
        if (enterType == EnterType.ColorMark && colorRemoval)
        {
            if (CheckIfTilesContainType(tiles, EnterType.ColorMark))
            {
                RemoveAllOfEntryType(tiles, EnterType.ColorMark);
            }
        
            return;
        }
        
        // all enter types in priority order
        List<EnterType> enterTypes = new List<EnterType>
        {
            EnterType.DigitMark,
            EnterType.CenterMark,
            EnterType.CornerMark,
            EnterType.ColorMark
        };

        // moving the given enter type to bottom of list, giving it top prioriy
        enterTypes.Remove(enterType);
        enterTypes.Insert(0, enterType);

        foreach (var type in enterTypes)
        {
            if (CheckIfTilesContainType(tiles, type))
            {
                // special case needed to update board
                if (type == EnterType.DigitMark)
                {
                    HandleRemoveNormalNumbers(tiles);
                }
                else
                {
                    RemoveAllOfEntryType(tiles, type);
                }
                
                return;
            }
        }
    }

    private void RemoveAllOfEntryType(List<TileBehaviour> tiles, EnterType type)
    {
        foreach (var tile in tiles)
        {
            tile.RemoveAllOfType(type);
        }
    }

    private bool CheckIfTilesContainType(List<TileBehaviour> selectedTiles, EnterType enterType)
    {
        foreach (var tile in selectedTiles)
        {
            // skip given clues
            if (SkipTile(tile, enterType)) continue;

            switch (enterType)
            {
                case EnterType.DigitMark:
                    if (tile.HasDigit)
                        return true;
                    break;
                
                case EnterType.CenterMark:
                    if (!tile.HasDigit && tile.CenterMarks.Count > 0)
                        return true;
                    break;
                
                case EnterType.CornerMark:
                    if (!tile.HasDigit && tile.CornerMarks.Count > 0)
                        return true;
                    break;
                
                case EnterType.ColorMark:
                    if (tile.ColorMarks.Count > 0)
                        return true;
                    break;
            }
        }
        
        return false;
    }

    private void OnSelectAllTilesWithNumber(TileBehaviour doubleClickTile)
    {
        if (doubleClickTile.HasDigit)
        {
            SelectAllTilesWithDigit(doubleClickTile);
            
            if (doubleClickTile.ColorMarks.Count > 0)
                SelectAllTilesWithColor(doubleClickTile);

            return;
        }
       
        if (doubleClickTile.CornerMarks.Count > 0)
            SelectAllTilesWithCorner(doubleClickTile);
        
        if (doubleClickTile.CenterMarks.Count > 0)
            SelectAllTilesWithCenter(doubleClickTile);
        
        if (doubleClickTile.ColorMarks.Count > 0)
            SelectAllTilesWithColor(doubleClickTile);
    }

    private void SelectAllTilesWithDigit(TileBehaviour doubleClickTile)
    {
        int number = doubleClickTile.number;
        if (number == 0) return;
        
        foreach (var tile in tileBehaviours)
        {
            if (tile.number == number)
                tile.Select();
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
                if (tile.CornerMarks.Contains(corner) && !tile.HasDigit)
                    tile.Select();
            }
        }
    }
    
    private void SelectAllTilesWithCenter(TileBehaviour doubleClickTile)
    {
        bool hasCenterDigit = doubleClickTile.CenterMarks.Count > 0;
        if (!hasCenterDigit) return;

        foreach (var center in doubleClickTile.CenterMarks)
        {
            foreach (var tile in tileBehaviours)
            {
                if (tile.CenterMarks.Contains(center) && !tile.HasDigit)
                    tile.Select();
            }
        }
    }
    
    private void SelectAllTilesWithColor(TileBehaviour doubleClickTile)
    {
        bool hasColorDigit = doubleClickTile.ColorMarks.Count > 0;
        if (!hasColorDigit) return;

        foreach (int color in doubleClickTile.ColorMarks)
        {
            foreach (var tile in tileBehaviours)
            {
                if (tile.ColorMarks.Contains(color))
                    tile.Select();
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
        bool allTilesHaveSameDigit = CheckIfAllTilesHaveNumber(selectedTiles, number, enterType);

        foreach (var tileBehaviour in selectedTiles)
        {
            if (SkipTile(tileBehaviour, enterType)) continue;

            EnterTileNumber(tileBehaviour, number, enterType, allTilesHaveSameDigit);
        }
    }

    private bool CheckIfAllTilesHaveNumber(List<TileBehaviour> selectedTiles, int number, EnterType enterType)
    {
        foreach (var tile in selectedTiles)
        {
            // skip given clues
            if (SkipTile(tile, enterType)) continue;
            
            // if not digit entry, skip tiles that already has digit
            if (enterType != EnterType.DigitMark && tile.HasDigit) continue;

            if (!tile.HasSameNumber(number, enterType))
                return false;
        }

        return true;
    }
    
    private void EnterTileNumber(TileBehaviour tileBehaviour, int number, EnterType enterType, bool sameNumber)
    {
        if (SkipTile(tileBehaviour, enterType))
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
