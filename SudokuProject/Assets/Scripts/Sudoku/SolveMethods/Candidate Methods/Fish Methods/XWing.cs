using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWing : FishMethod
{
    public override string GetName => "XWing";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchFish(grid, 2, out solveInformation);
    }
}


