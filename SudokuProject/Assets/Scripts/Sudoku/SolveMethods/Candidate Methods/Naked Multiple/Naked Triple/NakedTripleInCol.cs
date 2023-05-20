public class NakedTripleInCol : NakedMultiple
{
    public override string GetName => "Naked Triple In Col";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInCol(grid, 3, out removal);
    }
}