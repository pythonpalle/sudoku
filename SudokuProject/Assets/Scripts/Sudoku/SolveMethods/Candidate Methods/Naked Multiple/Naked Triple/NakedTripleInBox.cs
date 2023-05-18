public class NakedTripleInBox : NakedMultiple
{
    public override string GetName => "Naked Triple In Box";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInBox(grid, 3, out removal);
    }
}