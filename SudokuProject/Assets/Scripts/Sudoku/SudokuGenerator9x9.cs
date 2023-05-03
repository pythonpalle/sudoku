using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGenerator9x9
{
    private SudokuGrid9x9 grid;
    private System.Random random = new System.Random();
    private Stack<Move> moves;

    public SudokuGenerator9x9(SudokuGrid9x9 grid)
    {
        this.grid = grid;
        moves = new Stack<Move>();
    }

    public void Generate()
    {
        HandleNextGenerationStep();
    }

    private void HandleNextGenerationStep()
    {
        SudokuTile lowestEntropyTile = FindLowestEntropyTile();

        if (lowestEntropyTile.Entropy <= 0)
        {
            Debug.LogWarning($"Zero entropy tile at ({lowestEntropyTile.index.row},{lowestEntropyTile.index.col}");
            HandleBackTracking();
        }
        else
        {
            CollapseWaveFunction(lowestEntropyTile, 0);
        }
    }

    private void HandleBackTracking()
    {
        int lastEntropy;
        
        do
        {
            Move lastMove = moves.Pop();
            
            Debug.Log($"Backtracking, removing {lastMove.Number} " +
                      $"from ({lastMove.Tile.index.row}), ({lastMove.Tile.index.col})");
            
            lastMove.Tile.Number = 0;
            lastMove.Tile.AddCandidate(lastMove.Number);
            Propagate(lastMove.Tile.Number, lastMove.EffectedTiles, false);

            lastEntropy = moves.Peek().Tile.Entropy;
        } 
        while (lastEntropy < 2);
        
        //TODO:
        // backa tills hittat en tile med entropi 2 eller högre.
        // kolla om den kan vara en siffra som är högre en siffran den var tidigare.
        // Om den kan vara det : OK!
        // om inte, fortsätt while-loopen till nästa move
        
        Move moveToChange = moves.Pop();
        moveToChange.Tile.Number = 0;
        moveToChange.Tile.AddCandidate(moveToChange.Number);
        Propagate(moveToChange.Tile.Number, moveToChange.EffectedTiles, false);
        CollapseWaveFunction(moveToChange.Tile, moveToChange.Number);
    }

    private void CollapseWaveFunction(SudokuTile placeTile, int minValue)
    {
        placeTile.AssignLowestPossibleValue(minValue);
        List<SudokuTile> effectedTiles = FindEffectedTiles(placeTile);
        Propagate(placeTile.Number, effectedTiles);
        moves.Push(new Move(placeTile, placeTile.Number, effectedTiles));
    }

    private List<SudokuTile> FindEffectedTiles(SudokuTile tile)
    {
        int tileRow = tile.index.row;
        int tileCol = tile.index.col;

        List<SudokuTile> effectedTiles = new List<SudokuTile>();
        
        // Tiles in same row or column
        for (int i = 0; i < 9; i++)
        {
            effectedTiles.Add(grid.Tiles[i, tileCol]);
            effectedTiles.Add(grid.Tiles[tileRow, i]);
        }
        
        // Tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                effectedTiles.Add(grid.Tiles[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol]);
            } 
        }

        return effectedTiles;
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

        if (lowestEntropyTiles.Count <= 0)
        {
            Debug.LogWarning("All tiles are placed.");
            return null;
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

    private void Propagate(int number, List<SudokuTile> tilesToPropagate, bool remove = true)
    {
        foreach (var tile in tilesToPropagate)
        {
            if (remove) 
                tile.RemoveCandidate(number);
            else
                tile.AddCandidate(number);
        }
    }
}
