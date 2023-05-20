using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// NOTE: order important for comparison
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
        SetupConstructor(difficulty);
    }

    private void SetupConstructor(PuzzleDifficulty difficulty)
    {
        grid = new SudokuGrid9x9(true);
        _wfcGridSolver = new WFCGridSolver(difficulty);
        puzzleGridRemovalMoves = new Stack<Move>();
    }


    public void Generate(PuzzleDifficulty difficulty)
    {
        int attempts = 0;
        int maxAttempts = 10;
        
        do
        {
            SetupConstructor(difficulty);
            TryGenerate(difficulty);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning($"Too many attempts, a {difficulty} puzzle could not be created.");
                Debug.LogWarning($"Instead, the difficulty is {_wfcGridSolver.highestSuccessfulDifficulty}.");
                break;
            }

        } while (_wfcGridSolver.highestSuccessfulDifficulty != difficulty);
        
        Debug.Log($"The puzzle is finished after {attempts} attempts, Hurray!");
        Debug.Log($"Difficulty used: {_wfcGridSolver.highestSuccessfulDifficulty}");
        grid.PrintGrid();
        EventManager.GenerateGrid(grid);
    }

    private void TryGenerate(PuzzleDifficulty difficulty)
    {
        _wfcGridSolver.SetGrid(grid);
        bool solvedGridCreated = _wfcGridSolver.TrySolveGrid(false);

        grid = new SudokuGrid9x9(_wfcGridSolver.grid);
        grid.PrintGrid();

        bool puzzleCreated = false; 
        if (solvedGridCreated)
        {
            puzzleCreated = TryCreatePuzzleFromSolvedGrid(difficulty);
            Debug.Log($"Difficulty from last attempt: {_wfcGridSolver.highestSuccessfulDifficulty}");
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
        
        bool removeSymmetric = difficulty != PuzzleDifficulty.Hard;
        bool checkForHumanSolve = true;

        // while puzzle is not finished:
        while (!AllTilesVisited(visitedTiles))
        {
            iterationCount++;
            if (iterationCount > maxMoves)
            {
                Debug.LogWarning("Max iteration reached.");
                break;
            }
            
            SudokuGrid9x9 lastGrid = new SudokuGrid9x9(grid);
            
            //  1. Find lowest entropy tile
            TileIndex lowestEntropyTileIndex = FindLowestEntropyTileIndexFromUnVisited(visitedTiles);
            
            //  2. Remove it from grid, propagate
            RemoveFromGrid(visitedTiles, lowestEntropyTileIndex);
            
            // 3. Find symmetric neighbour, remove and propagate
            if (removeSymmetric)
                RemoveSymmetric(visitedTiles, lowestEntropyTileIndex);

            // 4: check to see if only one solution
            bool multipleSolutions = CheckIfMultipleSolutions(grid);

            // 5. Revert to last grid if multiple solutions OR if not humanly solvable
            if (multipleSolutions || (checkForHumanSolve && !HumanlySolvable(grid, difficulty)))
            {
                grid = new SudokuGrid9x9(lastGrid);
                // Debug.Log("Current grid state (after re-adding the clues):");
                // grid.PrintGrid();
                iterationCount++;
            }
        }

        return true;
    }

    private int GetMaxMovesFromDifficulty(PuzzleDifficulty difficulty)
    {
        switch (difficulty)
        {
            case PuzzleDifficulty.Easy:
                return 16;
            case PuzzleDifficulty.Medium:
                return 31;
            case PuzzleDifficulty.Hard:
                return 200;
            
            default:
                return 200;
        }
    }

    private bool HumanlySolvable(SudokuGrid9x9 sudokuGrid9X9, PuzzleDifficulty difficulty)
    {
        return _wfcGridSolver.HumanlySolvable(sudokuGrid9X9, difficulty);
    }
    
    private bool CheckIfMultipleSolutions(SudokuGrid9x9 sudokuGrid9X9)
    {
        return _wfcGridSolver.HasMultipleSolutions(sudokuGrid9X9);
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