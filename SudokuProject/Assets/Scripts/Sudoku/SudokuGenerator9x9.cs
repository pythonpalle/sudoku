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
        grid = new SudokuGrid9x9();
        
        solvedGridMoves = new Stack<Move>();
        puzzleGridRemovalMoves = new Stack<Move>();
    }

    public void Generate(bool makeSymmetricCollapse = false)
    {
        bool solvedGridCreated = TryCreateSolvedGrid(makeSymmetricCollapse);

        bool puzzleCreated = false; 
        if (solvedGridCreated)
        {
            Debug.Log("<<< SOLVED GRID COMPLETE, NOW REMOVING CLUES... >>>");
            puzzleCreated = TryCreatePuzzleFromSolvedGrid();
        }
    }

    private bool TryCreatePuzzleFromSolvedGrid()
    {
        bool[,] visitedTiles = new bool[9, 9];

        int counter = 0;

        // while puzzle is not finished:
        while (!AllTilesVisited(visitedTiles))
        {
            //  1. Find lowest entropy tile
            var lowestEntropyTile = FindLowestEntropyTile(visitedTiles);
            // Debug.Log("Lowest entropy: " + lowestEntropyTile.Entropy);
            
            //  2. Remove it from grid, propagate
            RemoveFromGrid(visitedTiles, lowestEntropyTile);
            
            // 3. Find symmetric neighbour, remove
            RemoveSymmetric(visitedTiles, lowestEntropyTile);

            grid.PrintGrid();
            counter++;

            if (counter > 81)
            {
                Debug.LogWarning("Counter Limit Exceeded");
                break;
            }
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

    private void RemoveSymmetric(bool[,] visitedTiles, SudokuTile lowestEntropyTile)
    {
        int row = lowestEntropyTile.index.row;
        int col = lowestEntropyTile.index.col;

        //  3. Find its symmetric neighbour , repeat 2
        bool middleTile = row == 4 && col == 4;
        if (!middleTile)
        {
            int symmetricRow = 8 - row;
            int symmetricCol = 8 - col;
            var symmetricTile = grid.Tiles[symmetricRow, symmetricCol];
                
            RemoveFromGrid(visitedTiles, symmetricTile);
        }
    }

    private void RemoveFromGrid(bool[,] visitedTiles, SudokuTile tile)
    {
        int tileNumber = tile.Number;
        tile.Number = 0;
        tile.AddCandidate(tileNumber);
        var effectedTiles = FindEffectedTiles(tile);
        AddStrikes(tileNumber, effectedTiles);
        tile.ResetStrikes(tileNumber);

        int row = tile.index.row;
        int col = tile.index.col;
        visitedTiles[row, col] = true;
            
        puzzleGridRemovalMoves.Push(new Move(tile, tileNumber, effectedTiles));
    }

    private void AddStrikes(int number, List<SudokuTile> effectedTiles)
    {
        foreach (var tile in effectedTiles)
        {
            tile.AddStrike(number);
        }
    }


    private void GetOtherRowsInBox(int boxRow, out int row1, out int row2)
    {
        row1 = row2 = 0;
        
        switch (boxRow)
        {
            case 0:
                row1 = 1;
                row2 = 2;
                break;
            
            case 1:
                //row1 = 0;
                row2 = 2;
                break;
            
            case 2:
                //row1 = 0;
                row2 = 1;
                break;
        }
    }

    private bool AllTilesVisited(bool[,] visitedTiles)
    {
        return visitedTiles.Cast<bool>().All(visited => visited);
    }

    private bool TryCreateSolvedGrid(bool makeSymmetric = false)
    {
        int iterations = 0;
        while (!solvedGridCompleted)
        {
            HandleNextGenerationStep(makeSymmetric);
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

    private void HandleNextGenerationStep(bool makeSymmetric)
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
                CollapseWaveFunction(lowestEntropyTile, makeSymmetric);
            }
        }
    }

    private void HandleBackTracking(bool makeSymmetric = false, bool secondPair = false)
    {
        int lastEntropy;
        Move moveToChange;
        int backtracks = 0;
        if (secondPair) backtracks++;
        bool backTrackedSymmetricNeighbours = false;
        
        do
        {
            Move lastMove = solvedGridMoves.Pop();
            
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
                makeSymmetric &&(
                backtracks % 2 == 0 ||              // symmetric backtracks two cells at the time 
                (lastMove.Tile.index.row == 4 &&    // unless its the middle cell
                 lastMove.Tile.index.col == 4));
        } 
        while (lastEntropy <= 1 && backTrackedSymmetricNeighbours);

        if (moveToChange.Tile.AssignLowestPossibleValue(moveToChange.Number))
        {
            Debug.Log($"...and replace it with a {moveToChange.Tile.Number}.");
            CollapseWaveFunction(moveToChange.Tile, false);
            grid.PrintGrid();
        }
        else
        {
            HandleBackTracking(makeSymmetric, true);
        }
        
    }

    private void CollapseWaveFunction(SudokuTile placeTile, bool includeSecondPair = true)
    {
        List<SudokuTile> effectedTiles = FindEffectedTiles(placeTile);
        effectedTiles = RemoveTilesWithMissingCandidate(effectedTiles, placeTile);

        Propagate(placeTile.Number, effectedTiles);
        solvedGridMoves.Push(new Move(placeTile, placeTile.Number, effectedTiles));

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
            solvedGridMoves.Push(new Move(symmetricNeighbourTile, symmetricNeighbourTile.Number, symmetricNeighbourEffectedTiles));
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
            var rowTile = grid.Tiles[i, tileCol];
            var colTile = grid.Tiles[tileRow, i];
            
            if (rowTile != tile) effectedTiles.Add(rowTile);
            if (colTile != tile) effectedTiles.Add(colTile);
        }
        
        // Tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                SudokuTile boxTile = grid.Tiles[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol];
                
                //if (!effectedTiles.Contains(boxTile))
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
    
    private SudokuTile FindLowestEntropyTile(bool[,] visited)
    {
        int lowestEntropy = FindLowestEntropy(visited);

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid.Tiles[row, col];
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
    
    private SudokuTile FindHighestEntropyTile(bool[,] visited)
    {
        int lowestEntropy = FindHighestEntropy(visited);

        List<SudokuTile> lowestEntropyTiles = new List<SudokuTile>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid.Tiles[row, col];
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
    
    private int FindHighestEntropy(bool[,] visited)
    {
        int highestValue = -1;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid.Tiles[row, col];
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
    
    private int FindLowestEntropy(bool[,] visited)
    {
        int lowestValue = int.MaxValue;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (visited[row, col]) continue;

                var tile = grid.Tiles[row, col];
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
