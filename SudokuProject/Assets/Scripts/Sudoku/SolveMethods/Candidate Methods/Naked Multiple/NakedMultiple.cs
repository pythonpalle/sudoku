
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public abstract class NakedMultiple : CandidateMethod
{
    protected bool TryFindMultipleInRow(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, true, out removal);
    }
    
    protected bool TryFindMultipleInCol(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, false, out removal);
    }
    
    protected bool TryFindMultipleInBox(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInBox(grid, multCount, out removal);
    }

    private bool CandidatesFromMultipleInRowCol(SudokuGrid9x9 grid, int multCount, bool fromRow, out CandidateRemoval removal)
    {
        List<SudokuTile> multTiles = new List<SudokuTile>();
        removal = new CandidateRemoval();

        for (int row = 0; row < 9; row++)
        {
            multTiles.Clear();
            
            for (int col = 0; col < 9; col++)
            {
                // invert index if checking column
                var tile = fromRow ? grid[row, col] : grid[col, row];

                if (tile.Used || tile.Entropy > multCount) 
                    continue;
                
                multTiles.Add(tile);
            }

            // try find multiples
            if (TryFindNakedMultipleFromTiles(grid, fromRow, multTiles, multCount, out removal))
                return true;

        }
        
        return false;
    }

    private bool TryFindEffectedTilesFromMultRowCol(SudokuGrid9x9 grid, List<TileIndex> multTiles, bool fromRow, out CandidateRemoval removal)
    {
        // todo: testa ej ha som out parameter
        
        
        var tileIndex = multTiles[0];
        
        int multRow = tileIndex.row;
        int multCol = tileIndex.col;

        var candidateSet = grid[tileIndex].Candidates;
        bool foundEffected = false;

        removal = new CandidateRemoval(new List<TileIndex>(), candidateSet);

        for (int i = 0; i < 9; i++)
        {
            var compareTile = fromRow ? grid[multRow, i] : grid[i, multCol];
            
            if (!ValidTile(compareTile, multTiles))
                continue;

            if (candidateSet.Overlaps(compareTile.Candidates))
            {
                removal.indexes.Add(compareTile.index);
                foundEffected = true;
            }
        }

        return foundEffected;
    }

    private bool TryFindNakedMultipleFromTiles(SudokuGrid9x9 grid, bool fromRow, List<SudokuTile> rightEntropyTiles, int multCount, out CandidateRemoval removal, bool boxCheck = false) 
    {
        //var multTiles = new List<TileIndex>(multCount);
        //HashSet<int> candidateSet = new HashSet<int>();

        removal = new CandidateRemoval();
        
        // cant have n tiles that share n candidates if only n-1 tiles exist
        if (rightEntropyTiles.Count < multCount)
            return false;
        

        int n = rightEntropyTiles.Count;
        int k = multCount;

        List<List<TileIndex>> potentialMultiples = new List<List<TileIndex>>();
        SudokuTile[] tempList = new SudokuTile[k];
        FindAllCombinations(potentialMultiples, rightEntropyTiles, tempList, 0, n-1, 0, k);

        Debug.Log("Potential multiples: " + potentialMultiples.Count);
        
        foreach (var multTileList in potentialMultiples)
        {
            if (!boxCheck && TryFindEffectedTilesFromMultRowCol(grid, multTileList, fromRow, out removal))
            {
                return true;
            }
                
            if (boxCheck && TryFindEffectedTilesFromBox(grid, multTileList, out removal))
            {
                return true;
            }
        }

        // for (int i = 0; i < rightEntropyTiles.Count; i++)
        // {
        //     multTiles.Clear();
        //
        //     var tile = rightEntropyTiles[i];
        //     candidateSet = tile.Candidates;
        //     
        //     for (int j = i+1; j < rightEntropyTiles.Count; j++)
        //     {
        //         var compareTile = rightEntropyTiles[j];
        //         var compareSet = compareTile.Candidates;
        //
        //         if (candidateSet.SetEquals(compareSet))
        //         {
        //             multTiles.Add(compareTile.index);
        //         }
        //     }
        //
        //     // multCount - 1 since the compare tile hasn't been added yet
        //     if (multTiles.Count == multCount - 1)
        //     {
        //         multTiles.Add(tile.index);
        //
        //         if (!boxCheck && TryFindEffectedTilesFromMultRowCol(grid, multTiles, fromRow, out removal))
        //         {
        //             return true;
        //         }
        //         
        //         if (boxCheck && TryFindEffectedTilesFromBox(grid, multTiles, out removal))
        //         {
        //             return true;
        //         }
        //     }
        // }

        return false;
    }

    private bool CandidatesFromMultipleInBox(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        List<SudokuTile> multTiles = new List<SudokuTile>();
        removal = new CandidateRemoval();
        
        foreach (var box in Boxes.boxes)
        {
            multTiles.Clear();

            for (int deltaRow = 0; deltaRow < 3; deltaRow++)
            {
                for (int deltaCol = 0; deltaCol < 3; deltaCol++)
                {
                    var tile = grid[box.row + deltaRow, box.col + deltaCol];
                    
                    if (tile.Used || tile.Entropy > multCount) 
                        continue;
                
                    multTiles.Add(tile);
                }
            }
            
            if (TryFindNakedMultipleFromTiles(grid, false, multTiles, multCount, out removal, true))
                return true;
        }

        return false;
    }
    
    private bool TryFindEffectedTilesFromBox(SudokuGrid9x9 grid, List<TileIndex> multTiles, out CandidateRemoval removal)
    {
        var tileIndex = multTiles[0];

        int topLeftBoxRow = tileIndex.row - tileIndex.row % 3;
        int topLeftBoxCol = tileIndex.col - tileIndex.col % 3;

        var candidateSet = grid[tileIndex].Candidates;
        bool foundEffected = false;

        removal = new CandidateRemoval(new List<TileIndex>(), candidateSet);

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                var compareTile = grid[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol];
            
                if (!ValidTile(compareTile, multTiles))
                    continue;

                if (candidateSet.Overlaps(compareTile.Candidates))
                {
                    removal.indexes.Add(compareTile.index);
                    foundEffected = true;
                }
            } 
        }
        
        return foundEffected;
    }
}