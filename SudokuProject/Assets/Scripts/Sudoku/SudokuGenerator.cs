using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGenerator
{
    private SudokuGrid grid;
    private System.Random random = new System.Random();

    public SudokuGenerator(SudokuGrid grid)
    {
        this.grid = grid;
    }

    public void Generate()
    {
        SudokuTile lowestEntropyTile = FindLowestEntropyTile();
    }

    private SudokuTile FindLowestEntropyTile()
    {
        int lowestEntropy = FindLowestEntropy();

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        foreach (var tile in grid.Tiles)
        {
            if (tile.Entropy == lowestEntropy)
                lowestEntropyTiles.Add(tile);
        }

        int randomIndex = random.Next(lowestEntropyTiles.Count);
        SudokuTile lowestEntropyTile = lowestEntropyTiles[randomIndex];
        return lowestEntropyTile;
    }

    private int FindLowestEntropy()
    {
        int lowestValue = int.MaxValue;
        SudokuTile lowestTile = null;

        foreach (var tile in grid.Tiles)
        {
            if (tile.Entropy < lowestValue)
                lowestValue = tile.Entropy;
        }

        return lowestValue;
    }
}
