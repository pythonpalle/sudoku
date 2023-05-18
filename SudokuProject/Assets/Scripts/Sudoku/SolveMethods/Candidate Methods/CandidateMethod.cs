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
}