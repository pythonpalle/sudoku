using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public struct SudokuGrid9x9
{
    const int size = 9;
    
    public SudokuGrid9x9(bool createTiles)
    {
        Tiles = new SudokuTile[size,size];
        SetupTiles();
    }

    public SudokuGrid9x9(SudokuGrid9x9 copyFrom)
    {
        Tiles = new SudokuTile[size,size];
        SetupTiles(copyFrom);
    }
    
    public SudokuTile[,] Tiles
    {
        get;
        private set;
    }

    public SudokuTile this[int row, int col]
    {
        get { return Tiles[row, col]; }
    }

    public SudokuTile this[TileIndex index]
    {
        get { return Tiles[index.row, index.col]; }
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
    
    private void SetupTiles(SudokuGrid9x9 copyFrom)
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                SetupTile(row, col, copyFrom[row,col]);
            }
        }
    }

    private void SetupTile(int row, int col)
    {
        Tiles[row, col] = new SudokuTile( row, col, 0, size);
    }
    
    private void SetupTile(int row, int col, SudokuTile copyFrom)
    {
        Tiles[row, col] = new SudokuTile(copyFrom);
    }

    public void SetNumberToIndex(TileIndex index, int number)
    {
        Tiles[index.row, index.col].Number = number;
    }
    
    public void SetNumberToIndex(int row, int col, int number)
    {
        Tiles[row, col].Number = number;
    }

    public bool AssignLowestPossibleValue(TileIndex index, int minNumber)
    {
        return Tiles[index.row, index.col].AssignLowestPossibleValue(minNumber);
    }
    
    public void AddCandidateToIndex(TileIndex index, int number)
    {
        Tiles[index.row, index.col].AddCandidate(number);
    }

    public void PrintGrid()
    {
        string gridString = String.Empty;
        
        for (int col = 0; col < size; col++)
        {
            string rowString = String.Empty;
            
            // box separator
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


    public void AddStrikeToIndex(TileIndex index, int number)
    {
        Tiles[index.row, index.col].AddStrike(number);
    }
    
    public void RemoveStrikeFromIndex(TileIndex index, int number)
    {
        Tiles[index.row, index.col].RemoveStrike(number);
    }

    public void ResetStrikesToIndex(TileIndex tileIndex, int tileNumber)
    {
        Tiles[tileIndex.row, tileIndex.col].ResetStrikes(tileNumber);
    }

    public void RemoveCandidateFromIndex(TileIndex index, int number)
    {
        Tiles[index.row, index.col].RemoveCandidate(number);
    }

    
}