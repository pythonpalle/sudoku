using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFish : FishMethod
{
    public override string GetName => "SwordFish";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchFish(grid, 3, out solveInformation);
    }
}