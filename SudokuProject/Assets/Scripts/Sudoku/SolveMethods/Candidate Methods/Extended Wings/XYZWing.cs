public class XYZWing : ExtendedWing
{
    public override string GetName => "XYZ-Wing";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindWingCandidates(grid, true, out solveInformation);
    }
}