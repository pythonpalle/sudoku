using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniquenessRectangle : CandidateMethod
{
    public override string GetName => "Uniqueness Rectangle";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        List<TileIndex> twoEntropyTiles = FindAllIndicesWithEntropy(grid, 2);

        removal = new CandidateRemoval();

        foreach (var tileIndex1 in twoEntropyTiles)
        {
            var tile1 = grid[tileIndex1];
            
            foreach (var tileIndex2 in twoEntropyTiles)
            {
                // don't compare tiles with same index
                if (tileIndex1 == tileIndex2)
                    continue;
                
                var tile2 = grid[tileIndex2];
                
                // must have same candidates
                if (!tile1.Candidates.SetEquals(tile2.Candidates))
                    continue;

                bool rowIntersect_tiles_1_2 = false;
                
                // check if tiles intersect row
                if (tileIndex1.row == tileIndex2.row)
                    rowIntersect_tiles_1_2 = true;
                
                // check if tiles intersect row
                else if (tileIndex1.col == tileIndex2.col)
                { }
                
                // if neither, continue
                else
                    continue;

                foreach (var tileIndex3 in twoEntropyTiles)
                {
                    // can't be same index
                    if (tileIndex2 == tileIndex3 || tileIndex1 == tileIndex3)
                        continue;
                    
                    var tile3 = grid[tileIndex3];
                
                    // must have same candidates
                    if (!tile2.Candidates.SetEquals(tile3.Candidates))
                        continue;
                    
                    // check if tiles intersect row or col. Else, continue.
                    if (rowIntersect_tiles_1_2 && tileIndex3.col == tileIndex1.col){}
                    else if (!rowIntersect_tiles_1_2 && tileIndex3.row == tileIndex1.row){}
                    else
                        continue;

                    // now we have a potential uniqueness triangle
                    int uniqueRow = rowIntersect_tiles_1_2 ? tileIndex3.row : tileIndex2.row;
                    int uniqueCol = rowIntersect_tiles_1_2 ? tileIndex2.col : tileIndex3.col;
                    
                    // no progress if the intersected tile already is used
                    var uniqueTile = grid[uniqueRow, uniqueCol];
                    if (uniqueTile.Used)
                        continue;

                    HashSet<int> removalCandidates = new HashSet<int>(grid[tileIndex1].Candidates);

                    // only deadly pattern if the intersected tile has both candidates
                    if (removalCandidates.IsSubsetOf(uniqueTile.Candidates))
                    {
                        removal.candidateSet = removalCandidates;
                        removal.indexes = new List<TileIndex> {uniqueTile.index};
                        grid.PrintGrid();
                        DebugRectangle(tileIndex1, tileIndex2, tileIndex3, uniqueTile.index, removalCandidates);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void DebugRectangle(TileIndex tile1, TileIndex tile2, TileIndex tile3, TileIndex uniqueTileIndex, HashSet<int> removalCandidates)
    {
        Debug.LogWarning("Uniqueness rectangle found:");
        
        Debug.Log("Corner 1: " + tile1);
        Debug.Log("Corner 2: " + tile2);
        Debug.Log("Corner 3: " + tile3);
        
        Debug.Log("Effected index: " + uniqueTileIndex);
        
        Debug.Log("Candidates: ");
        foreach (var cand in removalCandidates)
        {
            Debug.Log(cand);
        }
    }
}
