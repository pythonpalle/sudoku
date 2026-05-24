public class HiddenPair : HiddenMultiple
{
    public override string GetName => "Hidden Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    protected override int multCount => 2;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchHiddenMultiples(grid, out solveInformation);
    }
}