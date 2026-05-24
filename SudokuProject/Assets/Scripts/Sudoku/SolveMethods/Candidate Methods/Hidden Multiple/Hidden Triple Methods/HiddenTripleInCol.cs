public class HiddenTripleInCol : HiddenMultiple
{
    public override string GetName => "Hidden Triple In Col";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindMultipleInCol(grid, 3, out solveInformation);
    }
}