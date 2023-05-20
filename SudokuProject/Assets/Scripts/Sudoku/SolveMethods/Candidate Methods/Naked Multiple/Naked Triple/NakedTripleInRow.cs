public class NakedTripleInRow : NakedMultiple
{
    public override string GetName => "Naked Triple In Row";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInRow(grid, 3, out removal);
    }
}