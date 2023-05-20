public class HiddenTripleInRow : HiddenMultiple
{
    public override string GetName => "Hidden Triple In Row";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInRow(grid, 3, out removal);
    }
}