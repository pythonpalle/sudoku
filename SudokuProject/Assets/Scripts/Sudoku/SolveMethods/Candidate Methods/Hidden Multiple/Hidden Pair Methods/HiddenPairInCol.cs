public class HiddenPairInCol : HiddenMultiple
{
    public override string GetName => "Hidden Pair In Col";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInCol(grid, 2, out removal);
    }
}