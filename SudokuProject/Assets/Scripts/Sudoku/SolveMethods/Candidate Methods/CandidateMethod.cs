using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CandidateRemoval
{
    public List<TileIndex> indexes;
    public HashSet<int> candidateSet;

    public CandidateRemoval(List<TileIndex> indexes, HashSet<int> candidateSet)
    {
        this.indexes = indexes;
        this.candidateSet = candidateSet;
    }
}

public abstract class CandidateMethod
{
    public virtual bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();
        return false;
    }

    public virtual string GetName { get; set; }
    
    protected bool AllIndicesHaveSameRowCol(List<TileIndex> tileIndices, bool checkRow)
    {
        if (checkRow)
        {
            int tileRow = tileIndices[0].row;
            return tileIndices.All(tile => tile.row == tileRow);
        }
        else
        {
            int tileCol = tileIndices[0].col;
            return tileIndices.All(tile => tile.col == tileCol);
        }
    }
    
    protected bool ValidTile(SudokuTile compareTile, List<TileIndex> indices)
    {
        return !compareTile.Used && indices.All(index => index != compareTile.index);
    }
    
    protected void FindAllCombinations(List<List<TileIndex>> combinations, List<SudokuTile> tileList, SudokuTile[] tempList, 
        int start, int end, int index, int k)
    {
        // from https://www.geeksforgeeks.org/print-all-possible-combinations-of-r-elements-in-a-given-array-of-size-n/
        
        if (index >= k)
        {
            if (ValidCombination(tempList, k))
            {
                List<TileIndex> indices = GetIndicesFromTiles(tempList);
                combinations.Add(indices);
                Debug.Log("k: " + k + " indices: " + indices.Count);
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = tileList[i];
            FindAllCombinations(combinations, tileList, tempList, i + 1, end, index + 1, k);
        }
    }

    private List<TileIndex> GetIndicesFromTiles(SudokuTile[] tileList)
    {
        List<TileIndex> indices = new List<TileIndex>();
        foreach (var tile in tileList)
        {
            indices.Add(tile.index);
        }

        return indices;
    }

    private bool ValidCombination(SudokuTile[] tempList, int multCount)
    {
        HashSet<int> sharedCandidates = new HashSet<int>();
        foreach (var tile in tempList)
        {
            sharedCandidates.UnionWith(tile.Candidates);
        }
        
        bool rightAmountOfCandidates = sharedCandidates.Count == multCount;
        if (rightAmountOfCandidates)
        {
            Debug.Log("Here");
        }

        return sharedCandidates.Count == multCount;
    }
}