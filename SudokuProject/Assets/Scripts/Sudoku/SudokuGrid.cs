using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid
{
    protected int size;
    
    public Tile[,] Tiles
    {
        get;
        set;
    }

    public SudokuGrid(int size)
    {
        this.size = size;
        
        Tiles = new Tile[size,size];
        SetupTiles();
    }

    protected virtual void SetupTiles()
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                Tiles[row, col] = new Tile( new HashSet<int>(), 0, size);
                for (int i = 1; i < size; i++)
                {
                    Tiles[row, col].AddCandidate(i);
                }
            }
        }
    }
}
