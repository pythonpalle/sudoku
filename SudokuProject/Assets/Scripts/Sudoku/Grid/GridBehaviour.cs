using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Command;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public enum RemovalType
{
    None,
    Single,
    All
}

public class GridBehaviour : MonoBehaviour, IHasCommand
{
    private SudokuGrid9x9 grid;
    private List<SudokuGrid9x9> gridHistory;
    private GridSaver gridSaver;

    public TileBehaviour[,] tileBehaviours { get; private set; }= new TileBehaviour[9,9];

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
        
        CommandManager.instance.OnAddOneDigit += OnAddOneDigit;
        CommandManager.instance.OnAddMultipleDigits += OnAddMultipleDigits;
        CommandManager.instance.OnRemoveDigits += OnRemoveDigits;
        
        CommandManager.instance.OnAddMark += OnAddMark;
        CommandManager.instance.OnRemoveSingleMark += OnRemoveMark;

        CommandManager.instance.OnAddMarks += OnAddMarks;
        CommandManager.instance.OnRemoveAllMarks += OnRemoveAllMarks;
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
        gridPort.OnRequestTiles += OnRequestTiles;
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
        gridPort.OnRequestTiles -= OnRequestTiles;
        hintObject.OnHintFound -= OnHintFound;

        CommandManager.instance.OnAddOneDigit -= OnAddOneDigit;
        CommandManager.instance.OnAddMultipleDigits -= OnAddMultipleDigits;
        CommandManager.instance.OnRemoveDigits -= OnRemoveDigits;
        
        CommandManager.instance.OnAddMark -= OnAddMark;
        CommandManager.instance.OnRemoveSingleMark -= OnRemoveMark;
        
        CommandManager.instance.OnAddMarks -= OnAddMarks;
        CommandManager.instance.OnRemoveAllMarks -= OnRemoveAllMarks;
    }

    private void OnRemoveAllMarks(List<int> indexes, int enterType)
    {
        List<TileBehaviour> tiles = IntsToTiles(indexes);
        EnterType type = IntToEnterType(enterType);

        RemoveAllOfEntryType(tiles, type);
    }

    private void OnAddMarks(List<int> indexes, List<List<int>> markNumbers, int enterType)
    {
        Debug.Log($"Add {enterType} mark to {indexes.Count} indices!");
        
        List<TileBehaviour> tiles = IntsToTiles(indexes);
        EnterType type = IntToEnterType(enterType);

        for (int i = 0; i < indexes.Count; i++)
        {
            var tile = tiles[i];

            foreach (int mark in markNumbers[i])
            {
                EnterTileNumber(tile, mark, type, false);
                Debug.Log($"Add mark {mark} to tile {i}!");
            }
        }
    }

    private void OnAddOneDigit(List<int> indexes, int digit)
    {
        HandleRemoveContradictions();
        
        OnAddSingle(indexes, digit, EnterType.DigitMark);
        
        HandleAddContradictionsInList(IntsToTiles(indexes), digit);
        gridPort.UpdateContradictionStatus(GridHasContradiction());
        HandleCompletion();
    }

    private void OnAddMultipleDigits(List<int> indexes, List<int> newDigits)
    {
        HandleRemoveContradictions();

        List<TileBehaviour> tiles = IntsToTiles(indexes);

        for (var index = 0; index < tiles.Count; index++)
        {
            var tile = tiles[index];
            int digit = newDigits[index];
            EnterTileNumber(tile, newDigits[index], EnterType.DigitMark, false);
            HandleContradictionForTile(digit, tile);
        }
        
        gridPort.UpdateContradictionStatus(GridHasContradiction());
        HandleCompletion();
    }
    
    private void OnAddMark(List<int> indices, int number, int enterType)
    {
        OnAddSingle(indices, number, enterType);
    }

    private void OnAddSingle(List<int> indices, int number, int type)
    {
        OnAddSingle(indices, number, IntToEnterType(type));
    }
    
    private void OnAddSingle(List<int> indices, int number, EnterType type)
    {
        List<TileBehaviour> tiles = IntsToTiles(indices);
        
        for (var index = 0; index < tiles.Count; index++)
        {
            var tile = tiles[index];
            EnterTileNumber(tile, number, type, false);
        }
    }
    
    private void OnRemoveDigits(List<int> indices)
    {
        List<TileBehaviour> tiles = IntsToTiles(indices);
        
        HandleRemoveNormalNumbers(tiles);
    }
    
    private void OnRemoveMark(List<int> indeces, int number, int enterType)
    {
        List<TileBehaviour> tiles = IntsToTiles(indeces);
        EnterType type = IntToEnterType(enterType);

        foreach (var tile in tiles)
        {
            EnterTileNumber(tile, number, type, true);
        }
    }

    private EnterType IntToEnterType(int i)
    {
        return (EnterType) Enum.ToObject(typeof(EnterType), i);
    }

    private List<TileBehaviour> IntsToTiles(List<int> indexes)
    {
        List<TileBehaviour> tiles = new List<TileBehaviour>();
        
        for (int i = 0; i < indexes.Count; i++)
        {
            int index = indexes[i];
            int row = index / 9;
            int col = index % 9;

            var tile = tileBehaviours[row, col];
            tiles.Add(tile);
        }

        return tiles;
    }

    private void OnRequestTiles()
    {
        gridPort.tileBehaviours = tileBehaviours;
    }

    private void OnGridEnterFromUser(SudokuEntry entry)
    {
        if (entry.removal)
            OnRemoveEntry(entry);
        else
            OnNumberEnter(entry);
    }

    private void OnRequestGrid()
    {
        UpdateGridCandidates();
        gridPort.SendGridCopy(grid, tileBehaviours);
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
        
        //TODO: make import a valid command, or hard code work around for self created puzzles
         EventManager.CallNewCommand(null);
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

    private bool TrySkipPermanent(TileBehaviour tile, EnterType enterType)
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
        
        if (!TryEnterNumberToSelectedTiles(tiles, number, enterType))
            return;
        
        // switch (enterType)
        // {
        //     case EnterType.DigitMark:
        //         HandleRemoveContradictions();
        //         HandleAddContradictionsInList(tiles, number);
        //         HandleCompletion();
        //         gridPort.UpdateContradictionStatus(GridHasContradiction());
        //         break;
        // }
        //
        // EventManager.CallNewCommand(entry);
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

    private void OnRemoveEntry(SudokuEntry entry)
    {
        var tiles = entry.tiles;
        var enterType = entry.enterType;
        var colorRemoval = entry.colorRemoval;

        RemovalType removeAll = RemovalType.All;
        
        // special case for color removal, since it can't remove anything else
        if (enterType == EnterType.ColorMark && colorRemoval)
        {
            tiles = FilterEffectedOnly(tiles, removeAll, 0, EnterType.ColorMark);
            
            if (tiles.Count > 0)
            {
                Debug.Log($"Effected count: {tiles.Count}");

                var tileAsInt = TilesToInt(tiles);

                CreateRemoveAllMarksCommand(tiles, tileAsInt, EnterType.ColorMark);
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
            List<TileBehaviour> effected = FilterEffectedOnly(tiles, removeAll, 0, type);

            if (effected.Count > 0)
            {
                tiles = effected;
                Debug.Log($"Effected count: {tiles.Count}");

                // special case needed to update board
                if (type == EnterType.DigitMark)
                {
                    CreateRemoveDigitCommand(TilesToInt(tiles), GetPreviousDigits(tiles));
                }
                else
                {
                    CreateRemoveAllMarksCommand(tiles, TilesToInt(tiles), type);
                }
                
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
        
        Assert.IsTrue(somethingWasRemoved);

        return somethingWasRemoved;
    }

    private bool CheckIfTilesContainType(List<TileBehaviour> selectedTiles, EnterType enterType)
    {
        foreach (var tile in selectedTiles)
        {
            // skip given clues
            if (TrySkipPermanent(tile, enterType)) continue;

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

    private bool TryEnterNumberToSelectedTiles(List<TileBehaviour> selectedTiles, int number, EnterType enterType)
    {
        selectedTiles = selectedTiles.FindAll(t => !TrySkipPermanent(t, enterType));
        
        // if all selected tiles have the same number, remove the number 
        bool remove = selectedTiles.All(t => t.HasSameNumber(number, enterType));

        RemovalType removalType = remove ? RemovalType.Single : RemovalType.None;
        
        selectedTiles = FilterEffectedOnly(selectedTiles, removalType, number, enterType);
        
        if (selectedTiles.Count == 0) 
        {
            return false;
        }
        
        Debug.Log("Effected: " + selectedTiles.Count);
        CreateCommand(selectedTiles, enterType, removalType, number);
        return true;
    }

    private void CreateCommand(List<TileBehaviour> selectedTiles, EnterType enterType, RemovalType removalType, int number)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                CreateDigitCommand(selectedTiles, number, removalType);
                return;
            
            case EnterType.CornerMark:
            case EnterType.ColorMark:
            case EnterType.CenterMark:
                CreateMarkCommand(selectedTiles, number, enterType, removalType);
                return;
        }
    }

    private void CreateMarkCommand(List<TileBehaviour> tiles, int number, EnterType enterType, RemovalType removalType)
    {
        List<int> tilesAsIndices = TilesToInt(tiles);
        
        switch (removalType)
        {
            case RemovalType.None:
                CreateAddMarkCommand(number, tilesAsIndices, enterType);
                break;
            
            case RemovalType.Single:
                CreateRemoveSingleMarkCommand(number, tilesAsIndices, enterType);
                break;
            
            case RemovalType.All:
                CreateRemoveAllMarksCommand(tiles, tilesAsIndices, enterType);
                break;
        }
        
    }

    private void CreateRemoveAllMarksCommand(List<TileBehaviour> tiles, List<int> tilesAsIndices, EnterType enterType)
    {
        var removeAllMarksCommand = new RemoveAllMarksCommand()
        {
            effectedIndexes = tilesAsIndices,
            enterType = (int) enterType,
            previousMarks = GetPreviousMarks(tiles, enterType)
        };

        CommandManager.instance.ExecuteNewCommand(removeAllMarksCommand);
    }

    private static void CreateRemoveSingleMarkCommand(int number, List<int> tilesAsIndices, EnterType enterType)
    {
        var removeSingleMarkCommand = new RemoveSingleMarkCommand()
        {
            effectedIndexes = tilesAsIndices,
            enterType = (int) enterType,
            number = number
        };

        CommandManager.instance.ExecuteNewCommand(removeSingleMarkCommand);
    }

    private static void CreateAddMarkCommand(int number, List<int> tilesAsIndices, EnterType enterType)
    {
        var addMarkCommand = new AddMarkCommand
        {
            effectedIndexes = tilesAsIndices,
            enterType = (int) enterType,
            number = number,
        };

        CommandManager.instance.ExecuteNewCommand(addMarkCommand);
    }

    private List<List<int>> GetPreviousMarks(List<TileBehaviour> tiles, EnterType enterType)
    {
        List<List<int>> marksForTile = new List<List<int>>();
        
        foreach (var tile in tiles)
        {
            switch (enterType)
            {
                case EnterType.CenterMark:
                    marksForTile.Add(new List<int>(tile.CenterMarks));
                    break;
                
                case EnterType.CornerMark:
                    marksForTile.Add(new List<int>(tile.CornerMarks));
                    break;
                
                case EnterType.ColorMark:
                    marksForTile.Add(new List<int>(tile.ColorMarks));
                    break;
            }
        }

        return marksForTile;
    }

    private void CreateDigitCommand(List<TileBehaviour> selectedTiles, int number, RemovalType removalType)
    {
        List<int> tilesToInt = TilesToInt(selectedTiles);
        List<int> previousDigits = GetPreviousDigits(selectedTiles);

        if (removalType == RemovalType.None)
        {
            CreateAddDigitCommand(tilesToInt, previousDigits, number);
        }
        else
        {
            CreateRemoveDigitCommand(tilesToInt, previousDigits);
        }
    }

    private void CreateAddDigitCommand(List<int> tilesToInt, List<int> previousDigits, int number)
    {
        AddDigitCommand addDigitCommand = new AddDigitCommand
        {
            addedDigit = number,
            effectedIndexes = tilesToInt,
            previousGridDigits = previousDigits
        };
        
        CommandManager.instance.ExecuteNewCommand(addDigitCommand);
    }

    private void CreateRemoveDigitCommand(List<int> tilesToInt, List<int> previousDigits)
    {
        RemoveDigitCommand removeDigitCommand = new RemoveDigitCommand
        {
            effectedIndexes = tilesToInt,
            previousGridDigits = previousDigits
        };
        
        CommandManager.instance.ExecuteNewCommand(removeDigitCommand);
    }

    private List<int> GetPreviousDigits(List<TileBehaviour> selectedTiles)
    {
        List<int> previous = new List<int>();

        foreach (var tile in selectedTiles)
        {
            previous.Add(tile.number);
        }

        return previous;
    }

    private static List<int> TilesToInt(List<TileBehaviour> tiles)
    {
        List<int> ints = new List<int>();

        foreach (var tile in tiles)
        {
            ints.Add(tile.IndexInt);
        }

        return ints;
    }

    /// <summary>
    /// Filters out tiles in list _selectedTiles_ to only include tiles that were effected by a move. For instance,
    /// a permanent tile can never by effected by anything other than colors.
    /// </summary>
    /// <param name="selectedTiles"></param>
    /// <param name="number"></param>
    /// <param name="enterType"></param>
    /// <exception cref="NotImplementedException"></exception>
    private List<TileBehaviour> FilterEffectedOnly(List<TileBehaviour> selectedTiles, RemovalType removeType, int number, EnterType enterType)
    {
        return selectedTiles.FindAll(t => t.IsEffectedByEntry(number, enterType, removeType));
    }
    

    private void EnterTileNumber(TileBehaviour tileBehaviour, int number, EnterType enterType, bool remove)
    {
        tileBehaviour.TryUpdateNumber(number, enterType, remove);

        if (enterType == EnterType.DigitMark)
        {
            AddDigitToGrid(tileBehaviour, number, remove);
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
            HandleContradictionForTile(number, tile);
        }
    }

    private void HandleContradictionForTile(int number, TileBehaviour tile)
    {
        if (tile.Permanent || tile.number == 0) return;

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
        Debug.Log($"Remove {selectedTiles.Count} entires.");
        
        foreach (var tile in selectedTiles)
        {
            AddDigitToGrid(tile, 0, true);
            tile.TryUpdateNumber(0, EnterType.DigitMark, true);
        }
        
        HandleRemoveContradictions();
    }

    private int stateCounter = 0;

    public void OnNewCommand(SudokuEntry entry)
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
