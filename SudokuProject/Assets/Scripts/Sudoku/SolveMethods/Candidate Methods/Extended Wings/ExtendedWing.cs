using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class ExtendedWing : CandidateMethod
{
    protected bool TryFindWingCandidates(SudokuGrid9x9 grid, bool xyzWing, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();
        List<TileIndex> twoEntropyTiles = FindAllIndicesWithEntropy(grid, 2);
        
        if (twoEntropyTiles.Count < 3)
        {
            Debug.Log($"Only {twoEntropyTiles.Count} tiles with entropy 2.");
            return false;
        }
        
        // XYZ-Wings have three entropy in base
        List<TileIndex> threeEntropyTiles = FindAllIndicesWithEntropy(grid, 3);
        List<TileIndex> baseTiles = xyzWing ? threeEntropyTiles : twoEntropyTiles;

        // base and wings must have exactly
        // 1 shared candidate (XY-Wing), or
        // 2 shared candidates (XYZ-Wing)
        int commonCountBetweenBaseAndWing = xyzWing ? 2 : 1;

        foreach (var baseIndex in baseTiles)
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

                // base and first wing must have exactly
                // 1 shared candidate (XY-Wing), or
                // 2 shared candidates (XYZ-Wing)
                if (commonCandidates_Base_Wing1.Count != commonCountBetweenBaseAndWing)
                    continue;
                
                // Get the uncommon candidate(s) (symmetric difference)
                int sharedCandidatesCount = xyzWing ? 1 : 2;
                HashSet<int> difference_Base_Wing1 = new HashSet<int>(baseCandidates);
                difference_Base_Wing1.SymmetricExceptWith(wing1_Candidates);
                
                Assert.AreEqual(difference_Base_Wing1.Count, sharedCandidatesCount);
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

                    // wing 2 must be subset of base AND not be equal to wing 1 (XYZ-Wing)
                    if (xyzWing)
                    {
                        bool wing2_subset_of_base = wing2_Candidates.IsSubsetOf(baseCandidates);
                        bool wing1_equals_wing2 = wing2_Candidates.SetEquals(wing1_Candidates);

                        bool wing2_is_valid = wing2_subset_of_base && !wing1_equals_wing2;
                        if (!wing2_is_valid)
                            continue;
                    }
                    // wing2's candidate must be exactly the 2 uncommon ones (XY-wing),
                    else
                    {
                        if (!wing2_Candidates.SetEquals(difference_Base_Wing1))
                            continue;
                    }

                    HashSet<int> wing1_wing2_intersect_set = new HashSet<int>(wing2_Candidates);
                    wing1_wing2_intersect_set.IntersectWith(wing1_Candidates);
                    
                    Assert.AreEqual(1, wing1_wing2_intersect_set.Count);
                    
                    int intersectCandidate = wing1_wing2_intersect_set.Min();

                    if (TryFindIntersectTiles(grid, baseTile, wing1, wing2, intersectCandidate, xyzWing, out List<TileIndex> intersectIndices))
                    {
                        // should now have valid XYWing
                        // DebugWing(baseTile, wing1, wing2);
                        
                        removal.indexes = intersectIndices;
                        removal.candidateSet = wing1_wing2_intersect_set;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool TryFindIntersectTiles(SudokuGrid9x9 grid, SudokuTile baseTile, SudokuTile wing1, SudokuTile wing2,
        int intersectCandidate, bool zWing, out List<TileIndex> tileIndices)
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
            
            // xyz-wings need an extra restriction
            if (zWing && !TilesIntersect(intersectTile.index, baseIndex))
                continue;

            if (intersectTile.Candidates.Contains(intersectCandidate))
            {
                tileIndices.Add(intersectIndex);
            }
        }

        return tileIndices.Count > 0;
    }


    
    
    private void DebugWing(SudokuTile baseTile, SudokuTile wing1, SudokuTile wing2)
    {
        Debug.Log("Potential wing found: ");
        Debug.Log($"baseTile: {baseTile.index}");
        Debug.Log($"wing1: {wing1.index}");
        Debug.Log($"wing2: {wing2.index}");
    }
}