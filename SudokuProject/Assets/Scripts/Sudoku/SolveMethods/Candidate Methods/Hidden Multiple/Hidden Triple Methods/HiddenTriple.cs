public class HiddenTriple : HiddenMultiple
{
    public override string GetName => "Hidden Triple";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;

    protected override int multCount => 3;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchHiddenMultiples(grid, out solveInformation);
    }
}