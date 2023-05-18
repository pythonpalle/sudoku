public class PointingTripleBoxToRow : PointingMethod
{
    public override string GetName => "Pointing Triple Box To Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindBoxToRowCandidates(grid, 3, out removal);
    }
}