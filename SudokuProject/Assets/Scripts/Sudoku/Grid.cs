using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int size;
    private const int DEFAULT_SIZE = 9;
    
    public Tile[,] Tiles
    {
        get;
        set;
    }

    public Grid(int size = DEFAULT_SIZE)
    {
        this.size = size;
        
        Tiles = new Tile[size,size];
        SetupTiles();
    }

    private void SetupTiles()
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                if (size == DEFAULT_SIZE) Tiles[row, col] = new Tile();
                else
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
}
