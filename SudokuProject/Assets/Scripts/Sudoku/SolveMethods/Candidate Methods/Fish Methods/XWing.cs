using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWing : FishMethod
{
    public override string GetName => "XWing";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFish(grid, 2, out removal);
    }
}

public class XWingRow : FishMethod
{
    public override string GetName => "XWing Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFishInRow(grid, 2, out removal);
    }
}

public class XWingCol : FishMethod
{
    public override string GetName => "XWing Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFishInCol(grid, 2, out removal);
    }
}


