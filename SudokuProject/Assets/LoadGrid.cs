using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LoadGrid : MonoBehaviour
{
    [SerializeField] private List<LoadBox> LoadBoxes;

    private List<LoadTile> _loadTiles;

    private float timeOfLastShuffle;
    private float timeBetweenShuffles = 0.1f;
    
    Random rnd = new Random();

    private static List<string> numbers = new List<string>
    {
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        
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

    public void Shuffle()
    {
        // if (Time.time < timeOfLastShuffle + timeBetweenShuffles)
        //     return;

        Debug.Log("Shuffle...");

        foreach (var tile in _loadTiles)
        {
            int randIndex = rnd.Next(numbers.Count);
            string randomNumber = numbers[randIndex];

            tile.TileText.text = randomNumber;
        }
        

        timeOfLastShuffle = Time.time;
    }
}
