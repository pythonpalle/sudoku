public class HiddenPairInRow : HiddenMultiple
{
    public override string GetName => "Hidden Pair In Row";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindMultipleInRow(grid, 2, out solveInformation);
    }
}