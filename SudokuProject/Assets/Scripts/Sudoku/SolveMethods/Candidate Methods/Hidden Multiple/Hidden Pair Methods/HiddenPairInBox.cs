public class HiddenPairInBox : HiddenMultiple
{
    public override string GetName => "Hidden Pair In Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInBox(grid, 2, out removal);
    }
}