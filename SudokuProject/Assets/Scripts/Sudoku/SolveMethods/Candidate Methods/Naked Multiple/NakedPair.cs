public class NakedPair : NakedMultiple
{
    protected override int multCount => 2;
    public override string GetName => "Naked Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchNakedMultiples(grid, out solveInformation);
    }
}