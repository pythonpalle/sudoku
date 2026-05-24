public class NakedQuad : NakedMultiple
{
    protected override int multCount => 4;
    public override string GetName => "Naked Quad";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return SearchNakedMultiples(grid, out solveInformation);
    }
}