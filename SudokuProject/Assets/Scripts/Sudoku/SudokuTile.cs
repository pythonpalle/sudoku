using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuTile
{
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
            
            number = value;
        }
    }

    public HashSet<int> Candidates
    {
        get;
        set;
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

    public SudokuTile(HashSet<int> candidates, int number = 0, int highestNumber = 9)
    {
        this.highestNumber = highestNumber;
        Number = number;
        Candidates = candidates;
    }

    public SudokuTile() : this(new HashSet<int> {1, 2, 3, 4, 5, 6, 7, 8, 9}) {}
}