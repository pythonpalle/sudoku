using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGenerator9x9
{
    private const int GENERATION_ITERATION_LIMIT = 250;
    private System.Random random = new System.Random();

    private SudokuGrid9x9 grid;
    
    private Stack<Move> solvedGridMoves;
    private Stack<Move> puzzleGridRemovalMoves;

    private bool solvedGridCompleted => solvedGridMoves.Count >= 81;

    public SudokuGenerator9x9()
    {
        grid = new SudokuGrid9x9(true);
        
        solvedGridMoves = new Stack<Move>();
        puzzleGridRemovalMoves = new Stack<Move>();
    }

    public void Test()
    {
        SudokuGrid9x9 grid1 = new SudokuGrid9x9(true);
        grid1.Tiles[5, 5].Number = 5;
        //grid1.SetNumberToIndex(5,5,5);


        SudokuGrid9x9 grid2 = new SudokuGrid9x9(grid1);

        Debug.Log("grid 1: ");
        grid1.Tiles[5, 5].DebugTileInfo();
        
        Debug.Log("grid 2: ");
        grid2.Tiles[5, 5].DebugTileInfo();
        
        grid1.Tiles[5, 5].Number = 6;

        Debug.Log("grid 1: ");
        grid1.Tiles[5, 5].DebugTileInfo();
        
        Debug.Log("grid 2: ");
        grid2.Tiles[5, 5].DebugTileInfo();
    }

    public void Generate()
    {
        bool solvedGridCreated = TryCreateSolvedGrid();

        bool puzzleCreated = false; 
        if (solvedGridCreated)
        {
            Debug.Log("<<< SOLVED GRID COMPLETE, NOW REMOVING CLUES... >>>");
            puzzleCreated = TryCreatePuzzleFromSolvedGrid();
        }
    }

    private bool TryCreateSolvedGrid()
    {
        int iterations = 0;
        while (!solvedGridCompleted)
        {
            HandleNextGenerationStep();
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

    private void HandleNextGenerationStep()
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

    private void HandleBackTracking()
    {
        int lastEntropy;
        Move moveToChange;
        
        do
        {
            Move lastMove = solvedGridMoves.Pop();
            
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
        }
        else
        {
            HandleBackTracking();
        }
        
    }

    private void CollapseWaveFunction(TileIndex placeTileIndex)
    {
        List<TileIndex> effectedTileIndicies = FindEffectedTileIndicies(placeTileIndex);
        effectedTileIndicies = RemoveTilesWithMissingCandidate(effectedTileIndicies, placeTileIndex);

        int tileNumber = grid[placeTileIndex].Number;

        Propagate(tileNumber, effectedTileIndicies, true);
        solvedGridMoves.Push(new Move(placeTileIndex, tileNumber, effectedTileIndicies));
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
    
    private bool TryCreatePuzzleFromSolvedGrid()
    {
        bool[,] visitedTiles = new bool[9, 9];

        int counter = 0;

        // while puzzle is not finished:
        while (!AllTilesVisited(visitedTiles))
        {
            if (counter > 81)
            {
                Debug.LogError("Counter Limit Exceeded");
                break;
            }
            
            //  1. Find lowest entropy tile
            TileIndex lowestEntropyTileIndex = FindLowestEntropyTileIndexFromVisited(visitedTiles);
            //Debug.Log("Lowest entropy: " + grid[lowestEntropyTileIndex].Entropy);
            
            //  2. Remove it from grid, propagate
            RemoveFromGrid(visitedTiles, lowestEntropyTileIndex);
            
            // 3. Find symmetric neighbour, remove
            RemoveSymmetric(visitedTiles, lowestEntropyTileIndex);
            
            // 4. Find all solutions with brute force
            int solutionCount = FindAllSolutions(grid);

            if (solutionCount != 1 || !HumanlySolvable(grid))
            {
                MakeLatestMovesPermanentClues();
            }

            grid.PrintGrid();
            counter++;
        }
        
        // while puzzle is not finished:
        //  1. Find Highest Entropy Cell
        //  2. Remove it from grid, propagate
        //  3. Add that move
        //  4. Find its symmetric neighbour, repeat 2-3
        //  5. Check for number of solutions
        //  6. If not 1 solution, Make those cells permanent
        //  7. If 1 solution, check if humanly solvable
        //  8. if not humanly solvable, make those cells permanent

        return false;
    }

    private bool HumanlySolvable(SudokuGrid9x9 sudokuGrid9X9)
    {
        return true;
    }

    private void MakeLatestMovesPermanentClues()
    {
        TileIndex middleIndex = new TileIndex(4, 4);
        bool lastMoveMiddleTile = puzzleGridRemovalMoves.Peek().Index == middleIndex;
        
        MakeLatestMovePermanentClue();
        
        // middle tile has no symmetric neighbour 
        if (!lastMoveMiddleTile)
            MakeLatestMovePermanentClue();
    }

    private void MakeLatestMovePermanentClue()
    {
        Move latestMove = puzzleGridRemovalMoves.Pop();
        TileIndex latestIndex = latestMove.Index;
        int latestNumber = latestMove.Number;
        
        grid.SetNumberToIndex(latestIndex, latestNumber);
        
        // // not sure yet ?
        //grid.AddCandidateToIndex(tileIndex, tileNumber);
        
        var effectedIndecies = latestMove.EffectedTileIndecies;
        RemoveStrikes(latestNumber, effectedIndecies);

        // // antingen ingenting eller lägga till 3 strikes
        // grid.ResetStrikesToIndex(latestIndex, latestNumber);
    }

    

    private int FindAllSolutions(SudokuGrid9x9 grid9X9)
    {
        return 1;
    }

    private void RemoveSymmetric(bool[,] visitedTiles, TileIndex lowestEntropyTileIndex)
    {
        int row = lowestEntropyTileIndex.row;
        int col = lowestEntropyTileIndex.col;

        //  3. Find its symmetric neighbour , repeat 2
        bool middleTile = row == 4 && col == 4;
        if (!middleTile)
        {
            int symmetricRow = 8 - row;
            int symmetricCol = 8 - col;
            TileIndex symmetricTileIndex = grid[symmetricRow, symmetricCol].index;
                
            RemoveFromGrid(visitedTiles, symmetricTileIndex);
        }
    }

    private void RemoveFromGrid(bool[,] visitedTiles, TileIndex tileIndex)
    {
        int tileNumber = grid[tileIndex].Number;
        
        grid.SetNumberToIndex(tileIndex, 0);
        grid.AddCandidateToIndex(tileIndex, tileNumber);
        
        var effectedIndecies = FindEffectedTileIndicies(tileIndex);
        AddStrikes(tileNumber, effectedIndecies);
        grid.ResetStrikesToIndex(tileIndex, tileNumber);
        
        visitedTiles[tileIndex.row, tileIndex.col] = true;
        puzzleGridRemovalMoves.Push(new Move(tileIndex, tileNumber, effectedIndecies));
    }

    private void AddStrikes(int number, List<TileIndex> effectedTiles)
    {
        foreach (TileIndex index in effectedTiles)
        {
            grid.AddStrikeToIndex(index, number);
        }
    }
    
    private void RemoveStrikes(int number, List<TileIndex> effectedTiles)
    {
        foreach (TileIndex index in effectedTiles)
        {
            grid.RemoveStrikeFromIndex(index, number);
        }
    }
    

    private bool AllTilesVisited(bool[,] visitedTiles)
    {
        return visitedTiles.Cast<bool>().All(visited => visited);
    }
    
    private TileIndex FindLowestEntropyTileIndexFromVisited(bool[,] visited)
    {
        int lowestEntropy = FindLowestEntropyFromVisited(visited);

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid[row, col];
                if (tile.Entropy == lowestEntropy)
                {
                    lowestEntropyTiles.Add(tile);
                }
            }
        }

        int randomIndex = random.Next(lowestEntropyTiles.Count);
        SudokuTile lowestEntropyTile = lowestEntropyTiles[randomIndex];
        return lowestEntropyTile.index;
    }
    
    private SudokuTile FindHighestEntropyTileFromVisited(bool[,] visited)
    {
        int lowestEntropy = FindHighestEntropyFromVisited(visited);

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid[row, col];
                if (tile.Entropy == lowestEntropy)
                {
                    lowestEntropyTiles.Add(tile);
                }
            }
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
    
    private int FindHighestEntropyFromVisited(bool[,] visited)
    {
        int highestValue = -1;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid[row, col];
                if (tile.Entropy > highestValue)
                {
                    highestValue = tile.Entropy;
                    if (highestValue ==9)
                        return 9;
                }
            }
        }

        return highestValue;
    }
    
    private int FindLowestEntropyFromVisited(bool[,] visited)
    {
        int lowestValue = int.MaxValue;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid[row, col];
                if (tile.Entropy < lowestValue)
                {
                    lowestValue = tile.Entropy;
                    if (lowestValue ==0)
                        return 0;
                }
            }
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
