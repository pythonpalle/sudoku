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
        
        // set all permanent tiles
        int i = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++) 
            {
                var tile = _gridPort.tileBehaviours[row, col];
                
                dataHolder.numbers[i] = tile.number;
                dataHolder.permanent[i] = newSelfCreate || tile.Permanent;

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
        
        Debug.Log($"GEN TYPE {generatorPort.GenerationType}");
        
        Debug.Log("Created grid:");
        grid.PrintGrid();
        
        EventManager.GenerateGrid(grid);

        StartCoroutine(PerformCommandsRoutine(puzzleData, 0.02f));
    }

    private IEnumerator PerformCommandsRoutine(PuzzleDataHolder puzzleData, float f)
    {
        // wait for grid to finish generating
        yield return new WaitForSeconds(f);

        _gridPort.RequestGrid();
        yield return new WaitUntil(() => _gridPort.tileBehaviours != null);

        int totalCommandCount = puzzleData.commands.Count; 
        int savedCounter = puzzleData.commandCounter;
        
        bool showCommands = true;


        // execute all commands
        for (var index = 0; index < totalCommandCount; index++)
        {
            var commandData = puzzleData.commands[index];
            SudokuEntry entry = ToEntry(commandData);
            
            
            EventManager.GridEnterFromUser(entry);

            if (showCommands)
                yield return new WaitForEndOfFrame();
        }

        // temp solution, fix later
        CommandManager manager = FindObjectOfType<CommandManager>();
        
        // if saved counter is less then total command count, it means we have to go back a few command by undoing
        for (int i = savedCounter - 1; i < totalCommandCount; i++)
        {
            manager.CallUndo();
            
            if (showCommands)
                yield return new WaitForEndOfFrame();
        }
        
        GameStateManager.SetActive(true);
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