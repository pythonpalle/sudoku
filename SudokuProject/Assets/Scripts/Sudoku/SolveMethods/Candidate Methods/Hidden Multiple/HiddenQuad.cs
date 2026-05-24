public class HiddenQuad : HiddenMultiple
{
    public override string GetName => "Hidden Quad";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindMultipleInCol(grid, 4, out solveInformation) ||
         TryFindMultipleInRow(grid, 4, out solveInformation) ||
         TryFindMultipleInBox(grid, 4, out solveInformation);
    }
}