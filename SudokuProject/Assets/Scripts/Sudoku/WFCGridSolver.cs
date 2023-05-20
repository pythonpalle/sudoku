

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WFCGridSolver
{
    public WFCGridSolver(PuzzleDifficulty puzzleMAXDifficultyForHumanSolve)
    {
        moves = new Stack<Move>();
        maxDifficultyForHumanSolve = puzzleMAXDifficultyForHumanSolve;

        SetupSolveMethods();
    }

    private const int GENERATION_ITERATION_LIMIT = 16_384;
    
    private Stack<Move> moves;
    
    private PuzzleDifficulty maxDifficultyForHumanSolve;

    public PuzzleDifficulty highestSuccessfulDifficulty { get; private set; } = PuzzleDifficulty.Easy;
    private PuzzleDifficulty highestAttemptedDifficulty = PuzzleDifficulty.Easy;

    private List<DigitMethod> digitMethods;
    List<CandidateMethod> candidatesMethods;


    public SudokuGrid9x9 grid { get; protected set; }
    private System.Random random = new System.Random();
    
    
    List<SudokuGrid9x9> solvedGrids = new List<SudokuGrid9x9>();

    private bool gridFilled => grid.AllTilesAreUsed();

    private bool cancelSolve = false;

    public void SetGrid(SudokuGrid9x9 other)
    {
        grid = new SudokuGrid9x9(other);
    }
    
    private void SetupSolveMethods()
    {
        GetDigitMethods();
        GetCandidateMethods();
    }
    
    private void GetDigitMethods()
    {
        if (maxDifficultyForHumanSolve == PuzzleDifficulty.Easy)
        {
            digitMethods = new List<DigitMethod>{new HiddenSingleBox()};
        }
        else
        {
            digitMethods = new List<DigitMethod>
            {
                new NakedSingle(),
                
                new HiddenSingleColumn(), 
                new HiddenSingleInRow(),
                
                new HiddenSingleBox()
            };
        }
    }

    private void GetCandidateMethods()
    {
        candidatesMethods = new List<CandidateMethod>();

        switch (maxDifficultyForHumanSolve)
        {
            case PuzzleDifficulty.Easy:
                break;
            
            case PuzzleDifficulty.Medium:
                candidatesMethods = new List<CandidateMethod>
                {
                    // Pointing Pairs
                    new PointingPairBoxToRow(),
                    new PointingPairBoxToCol(),
                    
                    // Pointing Triples
                    new PointingTripleBoxToRow(),
                    new PointingTripleBoxToCol(),
                    
                    // Naked Pairs
                    new NakedPairInCol(),
                    new NakedPairInRow(),
                    
                    // Naked Triples
                    new NakedTripleInRow(),
                    new NakedTripleInCol(),
                };
                break;
            
            case PuzzleDifficulty.Hard:
                candidatesMethods = new List<CandidateMethod>
                {
                    // Pointing Pairs
                    new PointingPairRowToBox(),
                    new PointingPairColToBox(),
                    new PointingPairBoxToRow(),
                    new PointingPairBoxToCol(),
                    
                    // Pointing Triples
                    new PointingTripleRowToBox(),
                    new PointingTripleColToBox(),
                    new PointingTripleBoxToRow(),
                    new PointingTripleBoxToCol(),
                    
                    // Naked Pairs
                    new NakedPairInCol(),
                    new NakedPairInRow(),
                    new NakedPairInBox(),
                    
                    // Hidden pairs
                    new HiddenPairInBox(),
                    new HiddenPairInRow(),
                    new HiddenPairInCol(),
                    
                    // Naked Triples
                    new NakedTripleInRow(),
                    new NakedTripleInCol(),
                    new NakedTripleInBox(),
                    
                    // Hidden triples 
                    new HiddenTripleInBox(),
                    new HiddenTripleInRow(),
                    new HiddenTripleInCol(),
                    
                    // Naked Quad (includes row, col and box)
                    new NakedQuad(),
                    
                    // Hidden Quad (includes row, col and box)
                    new HiddenQuad(),
                    
                    // XWings
                    new XWingRow(),
                    new XWingCol(),
                    
                    // Swordfish
                    new SwordFishRow(),
                    new SwordFishCol(),
                    
                    // JellyFish
                    new JellyFishRow(),
                    new JellyFishCol(),
                    
                    // Extended Wings
                    new XYWing(),
                    new XYZWing(),
                };
                break;
        }
        
    }
    
    public bool HasMultipleSolutions(SudokuGrid9x9 originalGrid)
    {
        grid = new SudokuGrid9x9(originalGrid);
        solvedGrids = new List<SudokuGrid9x9>();
        moves = new Stack<Move>();
        cancelSolve = false;
        
        while (!gridFilled)
        {
            HandleNextSolveStep(true);

            // cancel solve means the algorithm has backtracked and tried all possible options 
            if (cancelSolve)
            {
                break;
            }
            
            if (solvedGrids.Count > 1)
            {
                return true;
            }
        }

        return solvedGrids.Count > 1;
    }

    public int GetSolutionCount(SudokuGrid9x9 originalGrid)
    {
        grid = new SudokuGrid9x9(originalGrid);
        FindAllSolutions();
        
        Debug.Log("Solutions found: " + solvedGrids.Count);
        return solvedGrids.Count;
    }
    
    public bool HumanlySolvable(SudokuGrid9x9 gridToCheck, PuzzleDifficulty difficulty)
    {
        grid = new SudokuGrid9x9(gridToCheck);
        
        int iterations = 0;
     
        while (!gridFilled)
        {
            iterations++;

            if (iterations > 90) 
            {
                Debug.LogError("Maximum iterations reached.");
                return false;
            }
            
            // start with digit methods, place digit directly
            if (TryProgressWithDigitMethods())
            {
                continue;
            }
            
            bool someMethodYieldProgress = TryProgressWithCandidateMethods();
            if (someMethodYieldProgress)
            {
                continue;
            }
            else
            {
                Debug.LogWarning("NOT SOLVABLE AT DIFFICULTY " + difficulty);
                return false;
            }
            
            
        }

        highestSuccessfulDifficulty = highestAttemptedDifficulty;
        return true;
    }

    private bool TryProgressWithDigitMethods()
    {
        foreach (var method in digitMethods)
        {
            if (method.TryFindDigit(grid, out TileIndex index, out int digit))
            {
                HandleNextSolveStep(index, digit);
                return true;
            }
        }
        
        return false;
    }

    private bool TryProgressWithCandidateMethods()
    {
        foreach (var method in candidatesMethods)
        {
            if (method.TryFindCandidates(grid, out CandidateRemoval removal))
            {
                RemoveCandidates(removal);
                UpdateAttemptedDifficult(method.Difficulty);
                return true;
            }
        }
        
        return false;
    }

    private void UpdateAttemptedDifficult(PuzzleDifficulty latestDifficult)
    {
        int latestDifficultValue = (int) latestDifficult;
        int highestAttemptedValue = (int) highestAttemptedDifficulty;

        if (latestDifficultValue > highestAttemptedValue)
        {
            highestAttemptedDifficulty = latestDifficult;
            Debug.Log("Highest Attempted difficult during solve: " + latestDifficult);
        }
    }

    private void DebugRemoval(CandidateRemoval removal, CandidateMethod method)
    {
        Debug.LogWarning("Found candidate(s) with: " + method.GetName);
        string digits = String.Empty;
        foreach (var candidate in removal.candidateSet)
        {
            digits += $"{candidate}, ";
        }
        Debug.Log($"Digit(s): {digits}");
                
        Debug.Log("Indices: ");
        foreach (var index in removal.indexes)
        {
            Debug.Log(index);
        }
    }

    private void RemoveCandidates(CandidateRemoval removal)
    {
        foreach (var index in removal.indexes)
        {
            foreach (var candidate in removal.candidateSet)
            {
                grid.RemoveCandidateFromIndex(index, candidate);
            }
        }
    }

    private void FindAllSolutions()
    {
        solvedGrids = new List<SudokuGrid9x9>();
        moves = new Stack<Move>();        
        
        TrySolveGrid(true);
        
        // if (solvedGrids.Count > 1)
        //     DebugAllSolutions();
    }

    private void DebugAllSolutions()
    {
        Debug.LogWarning("MULTIPLE SOLUTIONS FOUND. Solved grids count: " + solvedGrids.Count);
    }

    private bool TryAddNewSolution(SudokuGrid9x9 newGrid)
    {
        foreach (var solvedGrid in solvedGrids)
        {
            if (solvedGrid == newGrid)
            {
                Debug.Log("Solution already found, won't be added.");
                return false;
            }
        }
        
        solvedGrids.Add(newGrid);
        return true;
    }

    public bool TrySolveGrid(bool findAll = false)
    {
        cancelSolve = false;
        
        int iterations = 0;
        while (!gridFilled)
        {
            HandleNextSolveStep(findAll);

            if (cancelSolve)
            {
                return false;
            }
            

            iterations++;

            if (iterations >= GENERATION_ITERATION_LIMIT)
            {
                Debug.LogWarning("Maximum iterations reached, couldn't generate grid.");
                return false;
            }
        }
        
        return true;
    }

    private void HandleNextSolveStep(TileIndex nextIndex, int digit)
    {
        if (grid.AssignLowestPossibleValue(nextIndex, digit-1))
        {
            CollapseWaveFunction(nextIndex, false);
        }
        else
        {
            Debug.LogError("Can't assign value to tile");
        }
    }

    private void HandleNextSolveStep(bool findAll = false)
    {
        TileIndex lowestEntropyTileIndex = FindLowestEntropyTile(findAll);

        if (grid[lowestEntropyTileIndex].Entropy <= 0)
        {
            HandleBackTracking(findAll);
        }
        else
        {
            if (grid.AssignLowestPossibleValue(lowestEntropyTileIndex, 0))
            {
                CollapseWaveFunction(lowestEntropyTileIndex, findAll);
            }
        }
    }
    
    private void HandleBackTracking(bool findAllSolutions)
    {
        int lastEntropy;
        Move moveToChange;

        do
        {
            if (moves.Count <= 0)
            {
                cancelSolve = true;
                return;
            }
            
            Move lastMove = moves.Pop();
            
            grid.SetNumberToIndex(lastMove.Index, 0);
            grid.AddCandidateToIndex(lastMove.Index, lastMove.Number);
            Propagate(lastMove.Number, lastMove.EffectedTileIndecies, false);

            lastEntropy = grid[lastMove.Index].Entropy;
            moveToChange = lastMove;
        } 
        while (lastEntropy <= 1);

        if (grid.AssignLowestPossibleValue(moveToChange.Index, moveToChange.Number))
        {
            // Debug.Log($"...and replace it with a {grid[moveToChange.Index].Number}.");
            CollapseWaveFunction(moveToChange.Index, findAllSolutions); 
            //grid.PrintGrid();
            return;
        }
        else
        {
            HandleBackTracking(findAllSolutions);
        }
    }

    private void CollapseWaveFunction(TileIndex placeTileIndex, bool findAllSolutions = false)
    {
        List<TileIndex> effectedTileIndicies = FindEffectedTileIndicies(placeTileIndex);
        effectedTileIndicies = RemoveTilesWithMissingCandidate(effectedTileIndicies, placeTileIndex);

        int tileNumber = grid[placeTileIndex].Number;

        Propagate(tileNumber, effectedTileIndicies, true);
        moves.Push(new Move(placeTileIndex, tileNumber, effectedTileIndicies));

        if (findAllSolutions && gridFilled)
        {
            SudokuGrid9x9 solvedGrid = new SudokuGrid9x9(grid);
            TryAddNewSolution(solvedGrid);
            HandleBackTracking(findAllSolutions);
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

    private TileIndex FindLowestEntropyTile(bool findAll = false)
    {
        int lowestEntropy = FindLowestEntropy();

        List<TileIndex> lowestEntropyTiles = new List<TileIndex>();
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == lowestEntropy)
            {
                if (findAll) return tile.index;
                
                lowestEntropyTiles.Add(tile.index);
            }
        }

        if (lowestEntropyTiles.Count <= 0)
        {
            Debug.LogWarning("All tiles are placed.");
            return new TileIndex(0, 0);
        }

        TileIndex lowestEntropyTileIndex =lowestEntropyTiles[random.Next(lowestEntropyTiles.Count)];
        
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