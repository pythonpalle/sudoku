

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WFCGridSolver
{
    public WFCGridSolver()
    {
        moves = new Stack<Move>();
    }
    
    
    private const int GENERATION_ITERATION_LIMIT = 250;

    private Stack<Move> moves;
    private SudokuGrid9x9 grid;
    private System.Random random = new System.Random();

    private bool solvedGridCompleted => moves.Count >= 81;

    public int GetSolutionCount(SudokuGrid9x9 grid)
    {
        List<SudokuGrid9x9> solvedGrids = new List<SudokuGrid9x9>();

        foreach (var tile in grid.Tiles)
        {
            if (tile.Used) continue;

            foreach (var candidate in tile.Candidates)
            {
                SudokuGrid9x9 newGrid = new SudokuGrid9x9(grid);
                if (newGrid.AssignLowestPossibleValue(tile.index, candidate-1))
                {
                    CollapseWaveFunction(tile.index);
                    bool solved = TrySolveGrid(newGrid);
                    if (solved)
                    {
                        TryAddNewSoultion(solvedGrids, newGrid);
                    }
                }
            }
        }

        return solvedGrids.Count;
    }

    private void TryAddNewSoultion(List<SudokuGrid9x9> solvedGrids, SudokuGrid9x9 newGrid)
    {
        bool unique = true;
        
        foreach (var grid in solvedGrids)
        {
            if (grid == newGrid)
            {
                unique = false;
                return;
            }
        }
        
        solvedGrids.Add(newGrid);
    }

    public bool TrySolveGrid(SudokuGrid9x9 grid)
    {
        // intentional reference
        this.grid = grid;
        
        int iterations = 0;
        while (!solvedGridCompleted)
        {
            HandleNextSolveStep();
            grid.PrintGrid();

            iterations++;

            if (iterations >= GENERATION_ITERATION_LIMIT)
            {
                Debug.LogError("Maximum iterations reached, couldn't generate grid.");
                return false;
            }
        }

        return true;
    }
    
    private void HandleNextSolveStep()
    {
        TileIndex lowestEntropyTileIndex = FindLowestEntropyTile();
        
        if (grid[lowestEntropyTileIndex].Entropy <= 0)
        {
            Debug.LogWarning($"Zero entropy tile at ({lowestEntropyTileIndex.row},{lowestEntropyTileIndex.col})");
            HandleBackTracking();
        }
        else
        {
            if (grid.AssignLowestPossibleValue(lowestEntropyTileIndex, 0))
            {
                CollapseWaveFunction(lowestEntropyTileIndex);
            }
        }
    }
    
    private bool HandleBackTracking()
    {
        int lastEntropy;
        Move moveToChange;
        
        do
        {
            if (moves.Count <= 0)
            {
                Debug.Log("Out of moves, no solution found.");
                return false;
            }
            
            Move lastMove = moves.Pop();
            
            Debug.Log($"Backtracking, removing {lastMove.Number} " +
                      $"from ({lastMove.Index.row}), ({lastMove.Index.col})");
            
            grid.SetNumberToIndex(lastMove.Index, 0);
            grid.AddCandidateToIndex(lastMove.Index, lastMove.Number);
            
            Propagate(lastMove.Number, lastMove.EffectedTileIndecies, false);

            lastEntropy = grid[lastMove.Index].Entropy;
            moveToChange = lastMove;
            grid.PrintGrid();
        } 
        while (lastEntropy <= 1);

        if (grid.AssignLowestPossibleValue(moveToChange.Index, moveToChange.Number))
        {
            Debug.Log($"...and replace it with a {grid[moveToChange.Index].Number}.");
            CollapseWaveFunction(moveToChange.Index);
            grid.PrintGrid();
            return true;
        }
        else
        {
            return HandleBackTracking();
        }
    }

    private void CollapseWaveFunction(TileIndex placeTileIndex)
    {
        List<TileIndex> effectedTileIndicies = FindEffectedTileIndicies(placeTileIndex);
        effectedTileIndicies = RemoveTilesWithMissingCandidate(effectedTileIndicies, placeTileIndex);

        int tileNumber = grid[placeTileIndex].Number;

        Propagate(tileNumber, effectedTileIndicies, true);
        moves.Push(new Move(placeTileIndex, tileNumber, effectedTileIndicies));
    }
    
    private List<TileIndex> FindEffectedTileIndicies(TileIndex tileIndex)
    {
        int tileRow = tileIndex.row;
        int tileCol = tileIndex.col;

        List<TileIndex> effectedTiles = new List<TileIndex>();
        
        // Tiles in same row or column
        for (int i = 0; i < 9; i++)
        {
            var rowTile = grid[i, tileCol];
            var colTile = grid[tileRow, i];
            
            if (rowTile.index != tileIndex) 
                effectedTiles.Add(rowTile.index);
            
            if (colTile.index != tileIndex) 
                effectedTiles.Add(colTile.index);
        }
        
        // Tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                SudokuTile boxTile = grid[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol];
                
                if (boxTile.index != tileIndex)
                    effectedTiles.Add(boxTile.index);
            } 
        }

        return effectedTiles;
    }
    
    private List<TileIndex> RemoveTilesWithMissingCandidate(List<TileIndex> effectedTiles, TileIndex placeTileIndex)
    {
        List<TileIndex> filteredTiles = new List<TileIndex>();
        int number = grid[placeTileIndex].Number;

        foreach (TileIndex index in effectedTiles)
        {
            if (grid[index].Candidates.Contains(number))
            {
                filteredTiles.Add(index);
            }
        }

        return filteredTiles;
    }

    private TileIndex FindLowestEntropyTile()
    {
        int lowestEntropy = FindLowestEntropy();

        List<TileIndex> lowestEntropyTiles = new List<TileIndex>();
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == lowestEntropy)
                lowestEntropyTiles.Add(tile.index);
        }

        if (lowestEntropyTiles.Count <= 0)
        {
            Debug.LogWarning("All tiles are placed.");
            return new TileIndex(0, 0);
        }

        int randomIndex = random.Next(lowestEntropyTiles.Count);
        TileIndex lowestEntropyTileIndex = lowestEntropyTiles[randomIndex];
        return lowestEntropyTileIndex;
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

    private void Propagate(int number, List<TileIndex> tilesToPropagate, bool remove = true)
    {
        foreach (TileIndex index in tilesToPropagate)
        {
            if (remove)
                grid.RemoveCandidateFromIndex(index, number);
            else
                grid.AddCandidateToIndex(index, number);
        }
    }
    

}