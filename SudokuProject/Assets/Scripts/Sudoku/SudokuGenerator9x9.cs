using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGenerator9x9
{
    private SudokuGrid9x9 grid;
    private System.Random random = new System.Random();

    public SudokuGenerator9x9(SudokuGrid9x9 grid)
    {
        this.grid = grid;
    }

    public void Generate()
    {
        CollapseWaveFunction();
    }

    private void CollapseWaveFunction()
    {
        SudokuTile lowestEntropyTile = FindLowestEntropyTile();
        lowestEntropyTile.AssignRandomNumberFromCandidates();
        Propagate(lowestEntropyTile);
    }

    private SudokuTile FindLowestEntropyTile()
    {
        int lowestEntropy = FindLowestEntropy();

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == lowestEntropy)
                lowestEntropyTiles.Add(tile);
        }

        int randomIndex = random.Next(lowestEntropyTiles.Count);
        SudokuTile lowestEntropyTile = lowestEntropyTiles[randomIndex];
        return lowestEntropyTile;
    }

    private int FindLowestEntropy()
    {
        int lowestValue = int.MaxValue;

        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy < lowestValue)
                lowestValue = tile.Entropy;
        }

        return lowestValue;
    }
    
    private void Propagate(SudokuTile tile)
    {
        int tileRow = tile.index.row;
        int tileCol = tile.index.col;
        int tileNumber = tile.Number;
        
        // propagate row, col
        for (int i = 0; i < 9; i++)
        {
            grid.Tiles[i, tileCol].RemoveCandidate(tileNumber);  // propagate column
            grid.Tiles[tileRow, i].RemoveCandidate(tileNumber);  // propagate row
        }
        
        // propagate box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                grid.Tiles[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol]
                    .RemoveCandidate(tileNumber);
            } 
        }

    }
}
