using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class GridSaver : MonoBehaviour, IPopulatePuzzleData, ILoadPuzzleData
{
    [SerializeField] private GridPort _gridPort;
    [SerializeField] private GeneratorPort generatorPort;

    private void OnEnable()
    {
        AddListenersToSaveManager();

        if (generatorPort.GenerationType == GridGenerationType.loaded)
        {
            Invoke("LoadCurrentPuzzle", 0.02f);
        }
    }

    private void OnDisable()
    {
        RemoveListenersFromSaveManager();
    }
    
    private void LoadCurrentPuzzle()
    {
        SaveManager.LoadCurrentPuzzle();
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder, bool newSelfCreate)
    {
        _gridPort.RequestGrid();
        
        // set tile numbers and permanents
        int i = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++) 
            {
                var tile = _gridPort.tileBehaviours[row, col];
                
                dataHolder.numbers[i] = tile.number;
                dataHolder.permanent[i] = newSelfCreate || tile.Permanent;

                dataHolder.cornerMarks[i] = tile.CornerMarks;
                dataHolder.centerMarks[i] = tile.CenterMarks;
                dataHolder.colorMarks[i] = tile.ColorMarks;

                i++;
            } 
        }
    }

    public void LoadFromSaveData(PuzzleDataHolder puzzleData)
    {
        GameStateManager.SetActive(false);
        
        // create grid, populate it with permanent numbers
        SudokuGrid9x9 grid = new SudokuGrid9x9(false);

        var numbers = puzzleData.numbers;
        var permanents = puzzleData.permanent;

        for (int i = 0; i < 81; i++)
        {
            int number = numbers[i];
            
            if (!permanents[i] || number == 0)
                continue;
            
            int row = i / 9;
            int col = i % 9;
            TileIndex index = new TileIndex(row, col);

            grid.SetNumberToIndex(index, numbers[i]);
        }

        EventManager.GenerateGrid(grid);
        
        // Populate grid with non permanent digits and color, center and corner marks
        for (int i = 0; i < 81; i++)
        {
            // if digit for tile is not permanent, add it to grid
            int number = numbers[i];
            if (!permanents[i] || number == 0)
            {
                CommandManager.instance.AddDigitToTile(i, number);
            }
            
            // Get all marks for the tile, add them to grid
            Dictionary<EnterType, List<int>> marksForTile = GetAllMarksForTile(puzzleData, i);
            CommandManager.instance.AddAllMarksToTile(i, marksForTile);
        }

        _gridPort.RequestStatusUpdate();
        GameStateManager.SetActive(true);
    }

    private Dictionary<EnterType, List<int>> GetAllMarksForTile(PuzzleDataHolder puzzleData, int i)
    {
        Dictionary<EnterType, List<int>> marks = new Dictionary<EnterType, List<int>>();
        
        marks.Add(EnterType.CenterMark, puzzleData.centerMarks[i]);
        marks.Add(EnterType.CornerMark, puzzleData.cornerMarks[i]);
        marks.Add(EnterType.ColorMark, puzzleData.colorMarks[i]);

        return marks;
    }

    private SudokuEntry ToEntry(SerializedCommandData commandData)
    {
        List<TileBehaviour> tiles = _gridPort.GetTiles(commandData.tiles);
        EnterType enterType = (EnterType)Enum.ToObject(typeof(EnterType), commandData.enterType);
        
        SudokuEntry entry = new SudokuEntry(tiles, enterType, commandData.number, commandData.removal, commandData.colorRemoval);
        return entry;
    }

    void AddListenersToSaveManager()
    {
        SaveManager.AddLoadDataListener(this);
        SaveManager.AddPopulateDataListener(this);
    }

    void RemoveListenersFromSaveManager()
    {
        SaveManager.RemoveLoadDataListener(this);
        SaveManager.RemovePopulateDataListener(this);
    }
}