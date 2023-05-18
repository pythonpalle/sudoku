public class PointingTripleBoxToCol : PointingMethod
{
    public override string GetName => "Pointing Triple Box To Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindBoxToColCandidates(grid, 3, out removal);
    }
}