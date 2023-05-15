using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CandidateRemoval
{
    public List<TileIndex> indexes;
    public List<int> candidates;

    public CandidateRemoval(List<TileIndex> indexes, List<int> candidates)
    {
        this.indexes = indexes;
        this.candidates = candidates;
    }
}

public abstract class CandidateMethod
{
    public virtual bool TryFindCandidates(SudokuGrid9x9 inGrid, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();
        return false;
    }
}