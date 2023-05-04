using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGenerator9x9
{
    private SudokuGrid9x9 grid;
    private System.Random random = new System.Random();
    private Stack<Move> moves;

    public bool GenerationCompleted => moves.Count >= 81;

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
            lowestEntropyTile.AssignLowestPossibleValue(0);
            CollapseWaveFunction(lowestEntropyTile, 0);
        }
    }

    private void HandleBackTracking()
    {
        int lastEntropy;
        Move moveToChange;
        
        do
        {
            Move lastMove = moves.Pop();
            
            Debug.Log($"Backtracking, removing {lastMove.Number} " +
                      $"from ({lastMove.Tile.index.row}), ({lastMove.Tile.index.col})");
            lastMove.Tile.Number = 0;
            lastMove.Tile.AddCandidate(lastMove.Number);
            Propagate(lastMove.Number, lastMove.EffectedTiles, false);

            //lastEntropy = moves.Peek().Tile.Entropy;
            lastEntropy = lastMove.Tile.Entropy;
            moveToChange = lastMove;
            grid.PrintGrid();
        } 
        while (lastEntropy < 2);

        // Final undo
        // Move moveToChange = moves.Pop();
        //
        // Debug.Log($"Last backtrack, removing {moveToChange.Number} " +
        //           $"from ({moveToChange.Tile.index.row}), ({moveToChange.Tile.index.col})");
        // moveToChange.Tile.Number = 0;
        // moveToChange.Tile.AddCandidate(moveToChange.Number);
        // Propagate(moveToChange.Tile.Number, moveToChange.EffectedTiles, false);
        // grid.PrintGrid();

        if (moveToChange.Tile.AssignLowestPossibleValue(moveToChange.Number))
        {
            CollapseWaveFunction(moveToChange.Tile, moveToChange.Number);
            grid.PrintGrid();
        }
        else
        {
            HandleBackTracking();
        }
        
    }

    private void CollapseWaveFunction(SudokuTile placeTile, int minValue)
    {
        // placeTile.AssignLowestPossibleValue(minValue);
        List<SudokuTile> effectedTiles = FindEffectedTiles(placeTile);
        Propagate(placeTile.Number, effectedTiles);
        moves.Push(new Move(placeTile, placeTile.Number, effectedTiles));
    }

    private List<SudokuTile> FindEffectedTiles(SudokuTile tile)
    {
        //TODO: ta bara hänsyn till Tiles som fortfarande har den kandidaten
        
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
