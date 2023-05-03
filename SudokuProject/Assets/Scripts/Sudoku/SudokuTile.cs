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
    
    private System.Random random = new System.Random();

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

    public int Entropy => Candidates.Count;

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

    public SudokuTile(int row, int col, int number = 0, int highestNumber = 9)
    {
        index.row = row;
        index.col = col;

        Candidates = new HashSet<int>();
        for (int i = 1; i <= highestNumber; i++)
        {
            Candidates.Add(i);
        }
        
        this.highestNumber = highestNumber;
        Number = number;
    }
    
    public void AssignRandomNumberFromCandidates()
    {
        Number = Candidates.ElementAt(random.Next(Candidates.Count));
    }
}