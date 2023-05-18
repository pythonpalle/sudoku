public class NakedPairInRow : NakedMultiple
{
    public override string GetName => "Naked Pair In Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInRow(grid, 2, out removal);
    }
}