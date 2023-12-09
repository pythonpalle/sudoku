using System;
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
            Invoke("LoadCurrentPuzzle", 0.01f);
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
        // if populating from a self created grid, all tiles will be permenant
        
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
        
        Debug.Log("Created grid:");
        grid.PrintGrid();
        
        EventManager.GenerateGrid(grid); 

        // Tell command manager to execute all commands in order
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