using System;
using Saving;
using UnityEngine;

public class GridSaver : MonoBehaviour, IHasPuzzleData
{
    [SerializeField] private GridPort _gridPort;
    [SerializeField] private GeneratorPort generatorPort;

    private void OnEnable()
    {
        AddListenerToSaveManager();

        if (generatorPort.GenerationType == GridGenerationType.loaded)
        {
            Invoke("LoadCurrentPuzzle", 0.01f);
        }
    }

    private void LoadCurrentPuzzle()
    {
        SaveManager.LoadCurrentPuzzle();
    }

    private void OnDisable()
    {
        RemoveListenerFromSaveManager();
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder, GridGenerationType gridGenerationType)
    {
        bool allPermanent = gridGenerationType == GridGenerationType.empty;
        
        int i = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                dataHolder.numbers[i] = _gridPort.tileBehaviours[row, col].number;

                if (!allPermanent)
                {
                    dataHolder.permanent.Add(_gridPort.tileBehaviours[row, col].Permanent);
                }
                
                // // on a self created puzzle, every tile is considered permanent
                // dataHolder.permanent[i] = allPermanent ? true : _gridPort.tileBehaviours[row, col].Permanent;
                
                i++;
            }
        }
    }

    public void LoadFromSaveData(PuzzleDataHolder puzzleData)
    {
        // create grid, populate it with permanent numbers
        SudokuGrid9x9 grid = new SudokuGrid9x9(false);

        var numbers = puzzleData.numbers;

        for (int i = 0; i < 81; i++)
        {
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

    public void AddListenerToSaveManager()
    {
        SaveManager.AddPuzzleDataListener(this);
    }

    public void RemoveListenerFromSaveManager()
    {
        SaveManager.RemovePuzzleDataListener(this);
    }
}