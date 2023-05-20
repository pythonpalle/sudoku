using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : FishMethod
{
    public override string GetName => "JellyFish";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFish(grid, 4, out removal);
    }
}