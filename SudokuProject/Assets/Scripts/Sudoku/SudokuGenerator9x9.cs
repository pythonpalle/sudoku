using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PuzzleDifficulty
{
    Easy,
    Medium,
    Hard
}

public class SudokuGenerator9x9
{
    private System.Random random = new System.Random();

    private SudokuGrid9x9 grid;
    private WFCGridSolver _wfcGridSolver;
    
    private Stack<Move> puzzleGridRemovalMoves;
    
    public SudokuGenerator9x9(PuzzleDifficulty difficulty)
    {
        grid = new SudokuGrid9x9(true);
        this.difficulty = difficulty;

        _wfcGridSolver = new WFCGridSolver(difficulty);
        
        puzzleGridRemovalMoves = new Stack<Move>();
    }

    private PuzzleDifficulty difficulty;

    public void Generate(PuzzleDifficulty difficulty)
    {
        _wfcGridSolver.SetGrid(grid);
        bool solvedGridCreated = _wfcGridSolver.TrySolveGrid(false);

        grid = new SudokuGrid9x9(_wfcGridSolver.grid);
        grid.PrintGrid();

        bool puzzleCreated = false; 
        if (solvedGridCreated)
        {
            Debug.Log("<<< SOLVED GRID COMPLETE, NOW REMOVING CLUES... >>>");
            puzzleCreated = TryCreatePuzzleFromSolvedGrid(difficulty);
        }
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

    private bool TryCreatePuzzleFromSolvedGrid(PuzzleDifficulty difficulty)
    {
        bool[,] visitedTiles = new bool[9, 9];
        
        int iterationCount = 0;

        int maxMoves = GetMaxMovesFromDifficulty(difficulty);

        // while puzzle is not finished:
        while (!AllTilesVisited(visitedTiles))
        {
            SudokuGrid9x9 lastGrid = new SudokuGrid9x9(grid);
            
            //  1. Find lowest entropy tile
            TileIndex lowestEntropyTileIndex = FindLowestEntropyTileIndexFromUnVisited(visitedTiles);
            //TileIndex lowestEntropyTileIndex = GetRandomTileIndex(visitedTiles);
            
            //  2. Remove it from grid, propagate
            RemoveFromGrid(visitedTiles, lowestEntropyTileIndex);
            
            // 3. Find symmetric neighbour, remove and propagate
            RemoveSymmetric(visitedTiles, lowestEntropyTileIndex);

            // 4. Find all solutions with brute force
            int solutionCount = FindAllSolutions(grid);

            // 5. Revert to last grid if multiple solutions OR if not humanly solvable
            if (solutionCount != 1 || !HumanlySolvable(grid, difficulty))
            {
                grid = new SudokuGrid9x9(lastGrid);
                Debug.Log("Current grid state (after re-adding the clues):");
                grid.PrintGrid();
                iterationCount++;
            }

            iterationCount++;
            if (iterationCount > maxMoves)
                break;
        }

        Debug.Log("The puzzle is finished, Hurray!");
        EventManager.GenerateGrid(grid);

        return true;
    }

    private int GetMaxMovesFromDifficulty(PuzzleDifficulty difficulty)
    {
        switch (difficulty)
        {
            case PuzzleDifficulty.Easy:
                return 21;
            case PuzzleDifficulty.Medium:
                return 31;
            case PuzzleDifficulty.Hard:
                return 41;
            
            default:
                return 41;
        }
    }

    private bool HumanlySolvable(SudokuGrid9x9 sudokuGrid9X9, PuzzleDifficulty difficulty)
    {
        return _wfcGridSolver.HumanlySolvable(sudokuGrid9X9, difficulty);
    }

    private int FindAllSolutions(SudokuGrid9x9 grid9X9)
    {
        return _wfcGridSolver.GetSolutionCount(grid9X9);
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
    
    private TileIndex GetRandomTileIndex(bool[,] visitedTiles)
    {
        List<SudokuTile> tiles = new List<SudokuTile>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visitedTiles[row, col]) continue;
                
                tiles.Add(grid[row, col]);
                
            }
        }

        int randomIndex = random.Next(tiles.Count);
        SudokuTile randomTile = tiles[randomIndex];
        return randomTile.index;
    }
    
    private TileIndex FindLowestEntropyTileIndexFromUnVisited(bool[,] visited)
    {
        int lowestEntropy = FindLowestEntropyFromUnVisited(visited);

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
    
    private int FindLowestEntropyFromUnVisited(bool[,] visited)
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
}
