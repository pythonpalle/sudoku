using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TileIndex
{
    public int row;
    public int col;
}

public class SudokuTile
{
    public TileIndex index;
    public int Box { get; private set; }
    
    private int highestNumber { get; set; }

    private int number;
    public int Number
    {
        get => number;

        set{

            if (value < 0 || value > highestNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"The valid range is between 1 and {highestNumber}.");
            }
            
            RemoveCandidate(Number);
            number = value;
        }
    }

    public HashSet<int> Candidates
    {
        get;
        private set;
    }

    public Dictionary<int, int> Strikes
    {
        get;
        private set;
    }

    public bool Used => number > 0;

    public int Entropy => Candidates.Count;
    
    public SudokuTile(int row, int col, int number = 0, int highestNumber = 9)
    {
        index.row = row;
        index.col = col;

        Candidates = new HashSet<int>(highestNumber);
        Strikes = new Dictionary<int, int>(highestNumber);
        for (int i = 1; i <= highestNumber; i++)
        {
            Candidates.Add(i);
            Strikes.Add(i, 3);
        }
        
        this.highestNumber = highestNumber;
        Number = number;

        SetBox9x9();
    }

    private void SetBox9x9()
    {
        int boxRow = index.row / 3;
        int boxCol = index.col / 3;

        Box = boxRow * 3 + boxCol;
    }

    public void RemoveCandidate(int number)
    {
        Candidates.Remove(number);
    }
    
    public void AddCandidate(int number)
    {
        if (number > highestNumber)
        {
            throw new ArgumentOutOfRangeException(nameof(number),
                $"The valid range is between 1 and {highestNumber}.");
        }
        
        Candidates.Add(number);
    }

    public bool AssignLowestPossibleValue(int minValue)
    {
        if (Candidates.Count <= 0)
        {
            Debug.LogWarning("Cannot assign value to Tile, has zero entropy");
            return false;
        }
        
        if (minValue <= 0)
        {
            Number = Candidates.Min();
            return true;
        }
        else
        {
            int minPossibleCandidate = Int32.MaxValue;

            foreach (int value in Candidates)
            {
                if (value > minValue && value < minPossibleCandidate)
                {
                    minPossibleCandidate = value;
                }
            }

            if (minPossibleCandidate > highestNumber)
            {
                Debug.LogWarning("Cannot assign value to Tile, tried all options.");
                return false;
            }

            Number = minPossibleCandidate;
            return true;
        }
    }
    
    public void AddStrike(int number)
    {
        Strikes[number]--;
        if (Strikes[number] <= 0)
        {
            //Debug.Log($"Three strikes, Add candidate {number} to ({index.row},{index.col})");
            AddCandidate(number);
        }
    }
    
    public void ResetStrikes(int tileNumber)
    {
        Strikes[tileNumber] = 3;
    }

    public void DebugTileInfo()
    {
        Debug.Log($"Index: {index.row}, {index.col}");
        Debug.Log($"Number: {Number}");
        Debug.Log($"Box: {Box}");
        Debug.Log($"Entropy: {Entropy}");
    }


    
}