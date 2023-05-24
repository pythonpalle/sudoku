using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LoadGrid : MonoBehaviour
{
    [SerializeField] private List<LoadBox> LoadBoxes;
    [SerializeField] private Transform loadCircle;

    private LoadTile[,] loadTileMatriX = new LoadTile[9,9];
    Random rnd = new Random();

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            LoadBoxes[i].Setup(i);
            foreach (var tile in LoadBoxes[i].LoadTiles)
            {
                loadTileMatriX[tile.row, tile.col] = tile;
            }
        }
    }
    

    public void Shuffle(SudokuGrid9x9 grid, PuzzleDifficulty difficulty)
    {
        float numberOdds = GetNumberOddsFromDifficulty(difficulty);
        
        // added to the current one to change the grid slighlty
        int randomAddedNumber = rnd.Next(10);

        int iterations = 0;
        int halfGridCount = 40;
        
        loadCircle.Rotate(Vector3.forward, -30f);
        

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                bool useNumber = (float) rnd.NextDouble() < numberOdds;
                
                int symRow = 8 - row;
                int symCol = 8 - col;

                UpdateText(grid, row, col, useNumber, randomAddedNumber);
                UpdateText(grid, symRow, symCol, useNumber, randomAddedNumber);

                iterations++;
                if (iterations >= halfGridCount)
                    return;
            }
        }

    }

    private void UpdateText(SudokuGrid9x9 grid, int row, int col, bool useNumber, int randomNumber)
    {
        var tile = loadTileMatriX[row, col];

        if (useNumber)
        {
            int number = grid[row, col].Number;
            number = (number + randomNumber) % 9 + 1;
            tile.TileText.text = number.ToString();
        }
        else
        {
            tile.TileText.text = " ";
        }
    }

    private float GetNumberOddsFromDifficulty(PuzzleDifficulty difficulty)
    {
        switch (difficulty)
        {
            case PuzzleDifficulty.Simple:
                return 0.7f;
            
            case PuzzleDifficulty.Easy:
                return 0.4f;
            
            case PuzzleDifficulty.Medium:
                return 0.38f;
            
                default:
                return 0.35f;
        }
    }
}
