public class HiddenTripleInBox : HiddenMultiple
{
    public override string GetName => "Hidden Triple In Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInBox(grid, 3, out removal);
    }
}