public class HiddenTripleInBox : HiddenMultiple
{
    public override string GetName => "Hidden Triple In Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindMultipleInBox(grid, 3, out solveInformation);
    }
}