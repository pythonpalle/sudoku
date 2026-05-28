

using System;

public class XYWing : ExtendedWing
{
    public override string GetName => "XY-Wing";
    public override ExtendedWingType WingType => ExtendedWingType.XY;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchWingCandidates(grid, isXyzWing: false, out solveInformation);
    }
}