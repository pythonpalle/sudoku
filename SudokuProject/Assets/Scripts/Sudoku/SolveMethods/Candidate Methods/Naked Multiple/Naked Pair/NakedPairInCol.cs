public class NakedPairInCol : NakedMultiple
{
    public override string GetName => "Naked Pair In Col";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindMultipleInCol(grid, 2, out solveInformation);
    }
}