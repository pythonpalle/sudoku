public class HiddenTripleInCol : HiddenMultiple
{
    public override string GetName => "Hidden Triple In Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInCol(grid, 3, out removal);
    }
}