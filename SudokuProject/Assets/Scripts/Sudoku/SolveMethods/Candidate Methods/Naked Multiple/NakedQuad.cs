public class NakedQuad : NakedMultiple
{
    public override string GetName => "Naked Quad";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        int multCount = 4;
        return TryFindMultipleInCol(grid, multCount, out solveInformation) 
               || TryFindMultipleInRow(grid, multCount, out solveInformation) 
               || TryFindMultipleInBox(grid, multCount, out solveInformation);
    }
}