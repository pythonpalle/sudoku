using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LoadGrid : MonoBehaviour
{
    [SerializeField] private List<LoadBox> LoadBoxes;

    private List<LoadTile> _loadTiles;

    Random rnd = new Random();

    private static List<string> numbers = new List<string>
    {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
    };

    private void Awake()
    {
        _loadTiles = new List<LoadTile>();
        
        foreach (var box in LoadBoxes)
        {
            _loadTiles.AddRange(box.LoadTiles);
        }
    }

    public void Shuffle(PuzzleDifficulty difficulty)
    {
        Debug.Log("Shuffle...");

        float numberOdds = GetNumberOddsFromDifficulty(difficulty);

        foreach (var tile in _loadTiles)
        {
            bool useNumber = (float) rnd.NextDouble() < numberOdds;
            string tileString = " ";
            
            if (useNumber)
            {
                int randIndex = rnd.Next(numbers.Count);
                tileString = numbers[randIndex];
            }

            tile.TileText.text = tileString;
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
                return 0.3f;
            
                default:
                return 0.25f;
        }
    }
}
