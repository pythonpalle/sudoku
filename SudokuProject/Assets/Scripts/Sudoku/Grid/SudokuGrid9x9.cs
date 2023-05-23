using System;
using System.Collections.Generic;
using UnityEngine;

public struct SudokuGrid9x9
{
    const int size = 9;
    
    public SudokuGrid9x9(bool _)
    {
        Tiles = new SudokuTile[size,size];
        SetupTiles();
    }

    public SudokuGrid9x9(SudokuGrid9x9 copyFrom)
    {
        Tiles = new SudokuTile[size,size];
        SetupTiles(copyFrom);
    }
    
    public static bool operator==(SudokuGrid9x9 grid1, SudokuGrid9x9 grid2)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid1[row, col].Number != grid2[row, col].Number)
                    return false;
            }
        }

        return true;
    }
    
    public static bool operator!=(SudokuGrid9x9 grid1, SudokuGrid9x9 grid2)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid1[row, col].Number == grid2[row, col].Number)
                    return false;
            }
        }

        return true;
    }
    
    public override bool Equals(object other)
    {
        if(other == null)
            return false;

        if (other is SudokuGrid9x9 second)
        {
            return second == this;
        }

        return false;
    }


    public override int GetHashCode()
    {
        throw new NotImplementedException();
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
        
        for (int row = 0; row < size; row++)
        {
            string rowString = String.Empty;
            
            // box separator
            if (row % 3 == 0) gridString +=("-     -     -     -     -     -     -     -     -     -     -     -     -     -" + Environment.NewLine);

            for (int col = 0; col < size; col++)
            {
                if (col % 3 == 0) rowString += " | ";
                rowString += Tiles[row, col].Number + "       ";
            }

            gridString += rowString + Environment.NewLine;
        }

        gridString += Environment.NewLine;
        gridString += Environment.NewLine;
        
        foreach (var tile in Tiles)
        {
            gridString += tile.Used ? tile.Number : " ";
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


    public bool AllTilesAreUsed()
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                if (!Tiles[row, col].Used)
                    return false;
            }
        }

        return true;
    }

    public void DebugTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.DebugTileInfo();
        }
    }

    public void UpdateCandidatesForIndex(TileIndex index, HashSet<int> updatedCandidates)
    {
        Tiles[index.row, index.col].UpdateCandidates(updatedCandidates);
    }
}