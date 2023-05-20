using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

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
    public abstract string GetName { get; }

    public abstract PuzzleDifficulty Difficulty { get; }
    

    public virtual bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();
        return false;
    }

    
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
        
        if (index == k)
        {
            if (ValidCombination(tempList, k))
            {
                List<TileIndex> indices = GetIndicesFromTiles(tempList);
                combinations.Add(indices);
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = tileList[i];
            FindAllCombinations(combinations, tileList, tempList, i + 1, end, index + 1, k);
        }
    }
    
    protected void FindAllCombinations( List<MultiCombo> combinations, List<int> numbers,
        Dictionary<int, List<TileIndex>> candidateCount, int[] tempList, int start, int end, int index, int k)
    {
        if (index >= k)
        {
            if (ValidCombination(tempList, k, candidateCount, out List<TileIndex> indices))
            {
                HashSet<int> candidates = new HashSet<int>(tempList);

                MultiCombo combo = new MultiCombo(indices, candidates);

                combinations.Add(combo);
                //combinations.Add((indices, candidates));
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = numbers[i];
            FindAllCombinations(combinations, numbers, candidateCount, tempList, i + 1, end, index + 1, k);
        }
    }

    private bool ValidCombination(int[] tempList, int multCount, 
        Dictionary<int, List<TileIndex>> candidateCount, out List<TileIndex> combineIndicesForNumbers)
    {
        //List<TileIndex> combineIndicesForNumbers = new List<TileIndex>();
        combineIndicesForNumbers = new List<TileIndex>();

        foreach (int digit in tempList)
        {
            foreach (var tileIndex in candidateCount[digit])
            {
                if (!combineIndicesForNumbers.Contains(tileIndex))
                    combineIndicesForNumbers.Add(tileIndex);
            }
        }

        bool combineIndecesCountMatch = combineIndicesForNumbers.Count == multCount;
        return combineIndecesCountMatch;
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
        
        return sharedCandidates.Count == multCount;
    }
}