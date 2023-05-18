

using System.Collections.Generic;
using UnityEngine;

public class PointingPairBoxToRow : PointingMethod
{
    public override string GetName => "Pointing Pair Box To Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindBoxToRowCandidates(grid, 2, out removal);
    }
}