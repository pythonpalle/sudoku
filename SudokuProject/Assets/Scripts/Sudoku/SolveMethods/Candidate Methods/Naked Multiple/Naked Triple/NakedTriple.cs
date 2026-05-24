public class NakedTriple :  NakedMultiple
{
    protected override int multCount => 3;
    public override string GetName => "Naked Triple";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchNakedMultiples(grid, out solveInformation);
    }
}