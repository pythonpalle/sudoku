public class PointingPairBoxToCol : PointingMethod
{
    public override string GetName => "Pointing Pair Box To Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindBoxToColCandidates(grid, 2, out removal);
    }
}