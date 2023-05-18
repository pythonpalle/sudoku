public class PointingPairColToBox : PointingMethod
{
    public override string GetName => "Pointing Pair Col To Box";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindColToBoxCandidates(grid, 2, out removal);
    }
}