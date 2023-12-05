using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour, IHasCommand
{
    private SudokuGrid9x9 grid;
    private List<SudokuGrid9x9> gridHistory;

    private TileBehaviour[,] tileBehaviours = new TileBehaviour[9,9];

    [Header("Scriptable Objects")]
    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private HintObject hintObject;
    [SerializeField] private GridPort gridPort;
    
    [Header("Tile Animation Parent")]
    [SerializeField] private RectTransform tileAnimationParent;
    
    [Header("Grid Boxes")]
    [SerializeField] private List<GridBoxBehaviour> boxes;

    private void Awake()
    {
        gridPort.Reset();
    }

    private void Start()
    {
        SetupBoxes();
    }

    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnImportGrid += OnImportGrid;
        EventManager.OnTileIndexSet += OnTileIndexSet;
        
        EventManager.OnGridEnterFromUser += OnGridEnterFromUser;
        
        EventManager.OnNewCommand += OnNewCommand;
        EventManager.OnUndo += OnUndo;
        EventManager.OnRedo += OnRedo;

        EventManager.OnSelectAllTilesWithNumber += OnSelectAllTilesWithNumber;
        EventManager.OnSelectAllTiles += OnSelectAllTiles;

        selectionObject.OnRequestTile += OnRequestTile;

        gridPort.OnRequestGrid += OnRequestGrid;
        hintObject.OnHintFound += OnHintFound;
    }

    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnImportGrid -= OnImportGrid;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
        
        EventManager.OnGridEnterFromUser -= OnGridEnterFromUser; 

        EventManager.OnNewCommand -= OnNewCommand;
        EventManager.OnUndo -= OnUndo;
        EventManager.OnRedo -= OnRedo;
        
        EventManager.OnSelectAllTilesWithNumber -= OnSelectAllTilesWithNumber;
        EventManager.OnSelectAllTiles -= OnSelectAllTiles;

        selectionObject.OnRequestTile -= OnRequestTile;
        
        gridPort.OnRequestGrid -= OnRequestGrid;
        hintObject.OnHintFound -= OnHintFound;
    }

    private void OnGridEnterFromUser(SudokuEntry entry)
    {
        if (entry.removal)
            OnRemoveEntryEvent(entry);
        else
            OnNumberEnter(entry);
    }

    private void OnRequestGrid()
    {
        UpdateGridCandidates();
        gridPort.SendGridCopy(grid);
    }
    
    private void OnImportGrid(SudokuGrid9x9 importedGrid)
    {
        foreach (var tile in importedGrid.Tiles)
        {
            var tileBehaviour = tileBehaviours[tile.index.row, tile.index.col];
            tileBehaviour.TryUpdateNumber(tile.Number, EnterType.DigitMark, false);
        }

        foreach (var tileBehaviour in tileBehaviours)
        {
            if (CheckForContradiction(tileBehaviour))
                tileBehaviour.SetContradiction();
        }
        
        grid = importedGrid;
        EventManager.CallNewCommand();
    }

    private void UpdateGridCandidates()
    {
        foreach (var tile in grid.Tiles)
        {
            if (tile.Used) continue;
            HashSet<int> updatedCandidates = new HashSet<int> {1,2,3,4,5,6,7,8,9};

            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (CandidateIntersectsTile(tile, candidate))
                    updatedCandidates.Remove(candidate);
            }

            grid.UpdateCandidatesForIndex(tile.index, updatedCandidates);
        }
    }

    private bool CandidateIntersectsTile(SudokuTile tile, int candidate)
    {
        var tileIndex = tile.index;
        
        int tileRow = tileIndex.row;
        int tileCol = tileIndex.col;
        
        // Tiles in same row or column
        for (int i = 0; i < 9; i++)
        {
            var rowTile = grid[i, tileCol];
            if (Intersection(tileIndex, rowTile, candidate))
                return true;
            
            var colTile = grid[tileRow, i];
            if (Intersection(tileIndex, colTile, candidate))
                return true;
        }
        
        // Tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                SudokuTile boxTile = grid[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol];
                
                if (Intersection(tileIndex, boxTile, candidate))
                    return true;
            } 
        }

        return false;
    }

    private bool Intersection(TileIndex tileIndex, SudokuTile compareTile, int candidate)
    {
        return compareTile.index != tileIndex && compareTile.Number == candidate;
    }

    private void OnHintFound(TileIndex hintIndex)
    {
        Debug.Log($"Sending hint instruction to {hintIndex}.");
        TileBehaviour hintTileBehaviour = tileBehaviours[hintIndex.row, hintIndex.col];
        hintTileBehaviour.Hint();
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
            boxes[i].Setup(i, tileAnimationParent);
        }    
    }
    
    private void OnGridGenerated(SudokuGrid9x9 generatedGrid)
    {
        grid = generatedGrid;
        gridHistory = new List<SudokuGrid9x9>();
        
        SetupTileNumbers();
        EventManager.TilesSetup();
    }
    
    private void OnTileIndexSet(int row,  int col, TileBehaviour tileBehaviour)
    {
        tileBehaviours[row, col] = tileBehaviour;
    }
    
    private void OnNumberEnter(SudokuEntry entry)
    {
        var tiles = entry.tiles;
        var number = entry.number;
        var enterType = entry.enterType;
        
        HandleEnterNumberToSelectedTiles(tiles, number, enterType);
        
        switch (enterType)
        {
            case EnterType.DigitMark:
                HandleRemoveContradictions();
                HandleAddContradictionsInList(tiles, number);
                HandleCompletion();
                gridPort.UpdateContradictionStatus(GridHasContradiction());
                break;
        }
        
        EventManager.CallNewCommand();
    }
    
    private void HandleCompletion()
    {
        bool complete = CheckComplete();
        if (complete)
        {
            Debug.Log("You solved it, hurray!");
            EventManager.PuzzleComplete();
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

    private bool GridHasContradiction()
    {
        foreach (var tile in tileBehaviours)
        {
            if (tile.Contradicted)
                return true;
        }

        return false;
    }

    private void OnRemoveEntryEvent(SudokuEntry entry)
    {
        var tiles = entry.tiles;
        var enterType = entry.enterType;
        var colorRemoval = entry.colorRemoval;
        
        // special case for color removal, since it can't remove anything else
        if (enterType == EnterType.ColorMark && colorRemoval)
        {
            if (CheckIfTilesContainType(tiles, EnterType.ColorMark))
            {
                // only seen as new command if some tiles where effected
                if (RemoveAllOfEntryType(tiles, EnterType.ColorMark))
                {
                    EventManager.CallNewCommand();
                }
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
                    gridPort.UpdateContradictionStatus(GridHasContradiction());
                }
                else
                {
                    RemoveAllOfEntryType(tiles, type);
                }
                
                EventManager.CallNewCommand();
                return;
            }
        }
    }

    private bool RemoveAllOfEntryType(List<TileBehaviour> tiles, EnterType type)
    {
        bool somethingWasRemoved = false;
        foreach (var tile in tiles)
        {
            if (tile.TryRemoveAllOfType(type))
                somethingWasRemoved = true;
        }

        return somethingWasRemoved;
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
                try
                {
                    tileBehaviours[row, col].SetStartNumber(grid[row, col].Number);
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
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
            AddDigitToGrid(tileBehaviour, number, sameNumber);
        }
    }

    private void AddDigitToGrid(TileBehaviour tileBehaviour, int number, bool remove)
    {
        if (tileBehaviour.Permanent) return;
        
        int row = tileBehaviour.row;
        int col = tileBehaviour.col;

        if (remove)
            number = 0;

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
        int tileNumber = tile.number;
        if (tileNumber == 0)
            return false;
        
        var effectedTiles = GetEffectedTiles(tile);

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
            AddDigitToGrid(tile, 0, true);
            tile.TryUpdateNumber(0, EnterType.DigitMark, true);
        }
        
        HandleRemoveContradictions();
    }

    private int stateCounter = 0;

    public void OnNewCommand()
    {
        SudokuGrid9x9 newGrid = new SudokuGrid9x9(grid);
        
        while (gridHistory.Count > stateCounter)
        {
            gridHistory.RemoveAt(gridHistory.Count-1);
        }
        
        gridHistory.Add(newGrid);
        stateCounter++;
    }

    private void ChangeState(bool undo)
    {
        if (undo)
            stateCounter--;
        else
            stateCounter++;
        
        if (undo && stateCounter <= 0)
        {
            stateCounter = 1;
            return;
        }
        
        if (!undo && stateCounter > gridHistory.Count)
        {
            stateCounter --;
            return;
        }
        
        grid = new SudokuGrid9x9(gridHistory[stateCounter - 1]);
        
        gridPort.UpdateContradictionStatus(GridHasContradiction());
        //gridPort.OnContradictionStatusUpdate?.Invoke(GridHasContradiction());
    }

    private void OnUndo()
    {
        ChangeState(true);
    }
    
    private void OnRedo()
    {
        ChangeState(false);
    }
}
