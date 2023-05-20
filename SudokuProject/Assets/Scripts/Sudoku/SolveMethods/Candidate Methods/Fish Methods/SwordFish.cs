using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFish : FishMethod
{
    public override string GetName => "SwordFish";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFish(grid, 3, out removal);
    }
}