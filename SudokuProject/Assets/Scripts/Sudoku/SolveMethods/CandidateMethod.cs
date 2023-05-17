using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CandidateRemoval
{
    public List<TileIndex> indexes;
    public int candidate;

    public CandidateRemoval(List<TileIndex> indexes, int candidate)
    {
        this.indexes = indexes;
        this.candidate = candidate;
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
}