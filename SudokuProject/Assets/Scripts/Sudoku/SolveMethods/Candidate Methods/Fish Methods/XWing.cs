using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWing : FishMethod
{
    public override string GetName => "XWing";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFish(grid, 2, out solveInformation);
    }
}

public class XWingRow : FishMethod
{
    public override string GetName => "XWing Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInRow(grid, 2, out solveInformation);
    }
}

public class XWingCol : FishMethod
{
    public override string GetName => "XWing Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInCol(grid, 2, out solveInformation);
    }
}


