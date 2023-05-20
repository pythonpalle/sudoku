

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class XYWing : CandidateMethod
{
    public override string GetName => "XY-Wing";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();
        List<TileIndex> twoEntropyTiles = FindAllIndicesWithEntropy(grid, 2);
        
        if (twoEntropyTiles.Count < 3)
        {
            Debug.Log($"Only {twoEntropyTiles.Count} tiles with entropy 2.");
            return false;
        }

        foreach (var baseIndex in twoEntropyTiles)
        {
            var baseTile = grid[baseIndex];
            HashSet<int> baseCandidates = new HashSet<int>(baseTile.Candidates);

            foreach (var wing1_index in twoEntropyTiles)
            {
                var wing1 = grid[wing1_index];

                // can't be same tile
                if (wing1_index == baseIndex) continue;
                
                // wing1 must intersect with the base
                if (!TilesIntersect(baseTile.index, wing1.index))
                    continue;
                
                HashSet<int> wing1_Candidates = new HashSet<int>(wing1.Candidates);
                HashSet<int> commonCandidates_Base_Wing1 = new HashSet<int>(baseCandidates);
                commonCandidates_Base_Wing1.IntersectWith(wing1_Candidates);
                
                // base and first wing must have exactly 1 shared candidate
                if (commonCandidates_Base_Wing1.Count != 1)
                    continue;
                
                // Get the 2 uncommon candidates (symmetric difference)
                HashSet<int> difference_Base_Wing1 = new HashSet<int>(baseCandidates);
                difference_Base_Wing1.SymmetricExceptWith(wing1_Candidates);
                
                Assert.AreEqual(difference_Base_Wing1.Count, 2);
                Assert.IsFalse(wing1_Candidates.SetEquals(difference_Base_Wing1));
                Assert.IsFalse(baseCandidates.SetEquals(difference_Base_Wing1));

                foreach (var wing2_index in twoEntropyTiles)
                {
                    var wing2 = grid[wing2_index];

                    // can't be same tile
                    if (wing2.index == baseTile.index || wing1.index == baseTile.index) continue;
                    
                    // wing2 must also intersect with the base
                    if (!TilesIntersect(baseTile.index, wing2.index))
                        continue;
                    
                    HashSet<int> wing2_Candidates = new HashSet<int>(wing2.Candidates);
                    
                    // wing2's candidate must be exactly the 2 uncommon ones
                    if (!wing2_Candidates.SetEquals(difference_Base_Wing1))
                        continue;

                    HashSet<int> intersectSet = new HashSet<int>(wing2_Candidates);
                    intersectSet.IntersectWith(wing1_Candidates);
                    
                    Assert.AreEqual(1, intersectSet.Count);
                    
                    int intersectCandidate = intersectSet.Min();

                    if (TryFindIntersectTiles(grid, baseTile, wing1, wing2, intersectCandidate, out List<TileIndex> intersectIndices))
                    {
                        // should now have valid XYWing
                        DebugWing(baseTile, wing1, wing2);
                        
                        removal.indexes = intersectIndices;
                        removal.candidateSet = intersectSet;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool TryFindIntersectTiles(SudokuGrid9x9 grid, SudokuTile baseTile, SudokuTile wing1, SudokuTile wing2,
        int intersectCandidate, out List<TileIndex> tileIndices)
    {
        tileIndices = new List<TileIndex>();

        TileIndex baseIndex = baseTile.index;
        TileIndex wing1Index = wing1.index;
        TileIndex wing2Index = wing2.index;
        
        foreach (var intersectTile in grid.Tiles)
        {
            if (intersectTile.Used)
                continue;
            
            TileIndex intersectIndex = intersectTile.index;

            // wing can't intersect with itself
            if (intersectTile.index == wing1Index 
                || intersectTile.index == wing2Index
                || intersectTile.index == baseIndex)
                continue;
            
            if (!TilesIntersect(intersectTile.index, wing1Index))
                continue;
            
            if (!TilesIntersect(intersectTile.index, wing2Index))
                continue;

            if (intersectTile.Candidates.Contains(intersectCandidate))
            {
                tileIndices.Add(intersectIndex);
            }
        }

        return tileIndices.Count > 0;
    }


    private bool TilesIntersect(TileIndex index1, TileIndex index2)
    {
        // same row
        if (index1.row == index2.row)
            return true;
        
        // same col
        if (index1.col == index2.col)
            return true;
        
        int boxRowTile1 = index1.row - index1.row % 3;
        int boxRowTile2 = index2.row - index2.row % 3;
        
        int boxColTile1 = index1.col - index1.col % 3;
        int boxColTile2 = index2.col - index2.col % 3;
        
        // same box
        return (boxRowTile1 == boxRowTile2
                && boxColTile1 == boxColTile2);
    }

    private void DebugWing(SudokuTile baseTile, SudokuTile wing1, SudokuTile wing2)
    {
        Debug.Log("Potential wing found: ");
        Debug.Log($"baseTile: {baseTile.index}");
        Debug.Log($"wing1: {wing1.index}");
        Debug.Log($"wing2: {wing2.index}");
    }

    private List<SudokuTile> FindAllTilesWithEntropy(SudokuGrid9x9 grid, int entropy)
    {
        List<SudokuTile> entropyList = new List<SudokuTile>();
        
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == entropy)
                entropyList.Add(tile);
        }

        return entropyList;
    }
    
    private List<TileIndex> FindAllIndicesWithEntropy(SudokuGrid9x9 grid, int entropy)
    {
        List<TileIndex> entropyList = new List<TileIndex>();
        
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == entropy)
                entropyList.Add(tile.index);
        }

        return entropyList;
    }
}