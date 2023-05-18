public class NakedPairInCol : NakedMultiple
{
    public override string GetName => "Naked Pair In Col";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInCol(grid, 2, out removal);
    }
}