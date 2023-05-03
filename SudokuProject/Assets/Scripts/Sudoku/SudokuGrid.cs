using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGrid
{
    protected int size;
    
    public SudokuTile[,] Tiles
    {
        get;
        set;
    }

    public SudokuGrid(int size)
    {
        this.size = size;
        
        Tiles = new SudokuTile[size,size];
        SetupTiles();
    }

    private void SetupTiles()
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                SetupTile(row, col);
            }
        }
    }

    private void SetupTile(int row, int col)
    {
        Tiles[row, col] = new SudokuTile( row, col, 0, size);
        for (int i = 1; i < size; i++)
        {
            Tiles[row, col].AddCandidate(i);
        }

        Tiles[row, col].index.row = row;
        Tiles[row, col].index.col = col;
    }

    public void PrintGrid()
    {
        string gridString = String.Empty;
        
        for (int col = 0; col < size; col++)
        {
            string rowString = String.Empty;
            
            if (col % 3 == 0) gridString +=("-     -     -     -     -     -     -     -     -     -     -     -     -     -" + Environment.NewLine);

            for (int row = 0; row < size; row++)
            {
                if (row % 3 == 0) rowString += " | ";
                rowString += Tiles[row, col].Number + "       ";
            }

            gridString += rowString + Environment.NewLine;
        }
        
        Debug.Log(gridString);
    }
}
