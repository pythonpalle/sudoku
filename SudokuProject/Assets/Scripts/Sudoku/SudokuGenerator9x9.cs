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
        //AddGridState();
    }

    private void AddGridState()
    {
        //TODO:Make deep copy of state
        
        SudokuGrid9x9 state = new SudokuGrid9x9();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                SudokuTile stateTile = state.Tiles[row, col];
                SudokuTile gridTile = state.Tiles[row, col];

                //stateTile.Candidates = new HashSet<int>(gridTile.Candidates);
                stateTile.Number = gridTile.Number;

            }
        }
        //gridStates.Push(state);
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
            Debug.LogWarning($"Zero entropy tile at ({lowestEntropyTile.index.row},{lowestEntropyTile.index.col})");
            HandleBackTracking();
        }
        else
        {
            if (lowestEntropyTile.AssignLowestPossibleValue(0))
            {
                CollapseWaveFunction(lowestEntropyTile);
            }
        }
    }

    private void HandleBackTracking(bool secondPair = false)
    {
        int lastEntropy;
        Move moveToChange;
        int backtracks = 0;
        if (secondPair) backtracks++;
        bool backTrackedSymmetricNeighbours = false;
        
        do
        {
            Move lastMove = moves.Pop();
            
            Debug.Log($"Backtracking, removing {lastMove.Number} " +
                      $"from ({lastMove.Tile.index.row}), ({lastMove.Tile.index.col})");
            lastMove.Tile.Number = 0;
            lastMove.Tile.AddCandidate(lastMove.Number);
            Propagate(lastMove.Number, lastMove.EffectedTiles, false);

            lastEntropy = lastMove.Tile.Entropy;
            moveToChange = lastMove;
            grid.PrintGrid();
            backtracks++;

            backTrackedSymmetricNeighbours =
                backtracks % 2 == 0 ||
                (lastMove.Tile.index.row == 4 &&
                 lastMove.Tile.index.col == 4);
        } 
        while (lastEntropy < 2 && backTrackedSymmetricNeighbours);

        if (moveToChange.Tile.AssignLowestPossibleValue(moveToChange.Number))
        {
            Debug.Log($"...and replace it with a {moveToChange.Tile.Number}.");
            CollapseWaveFunction(moveToChange.Tile, false);
            grid.PrintGrid();
        }
        else
        {
            HandleBackTracking(true);
        }
        
    }

    private void CollapseWaveFunction(SudokuTile placeTile, bool includeSecondPair = true)
    {
        List<SudokuTile> effectedTiles = FindEffectedTiles(placeTile);
        effectedTiles = RemoveTilesWithMissingCandidate(effectedTiles, placeTile);

        Propagate(placeTile.Number, effectedTiles);
        moves.Push(new Move(placeTile, placeTile.Number, effectedTiles));

        if (!includeSecondPair) return;

        int symmetricRow = 8 - placeTile.index.row;
        int symmetricCol = 8 - placeTile.index.col;
        
        // middle tile has no symmetric neighbour
        if (symmetricRow == 4 && symmetricCol == 4)
        {
            Debug.Log("Middle Tile placed!");
            return;
        }
        
        SudokuTile symmetricNeighbourTile = grid.Tiles[symmetricRow, symmetricCol];
        if (symmetricNeighbourTile.AssignLowestPossibleValue(0))
        {
            List<SudokuTile> symmetricNeighbourEffectedTiles = FindEffectedTiles(symmetricNeighbourTile);
            symmetricNeighbourEffectedTiles = RemoveTilesWithMissingCandidate(symmetricNeighbourEffectedTiles, symmetricNeighbourTile);

            Propagate(symmetricNeighbourTile.Number, symmetricNeighbourEffectedTiles);
            moves.Push(new Move(symmetricNeighbourTile, symmetricNeighbourTile.Number, symmetricNeighbourEffectedTiles));
        }
        else
        {
            HandleBackTracking(false);
        }

        
    }

    private List<SudokuTile> RemoveTilesWithMissingCandidate(List<SudokuTile> effectedTiles, SudokuTile placeTile)
    {
        List<SudokuTile> filteredTiles = new List<SudokuTile>();
        int number = placeTile.Number;

        foreach (var tile in effectedTiles)
        {
            if (tile.Candidates.Contains(number))
                filteredTiles.Add(tile);
        }

        return filteredTiles;
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
