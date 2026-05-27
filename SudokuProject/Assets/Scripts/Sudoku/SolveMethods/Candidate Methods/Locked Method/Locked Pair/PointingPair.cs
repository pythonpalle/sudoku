public class PointingPair : PointingMethod
{
    public override string GetName => "Pointing Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        if (SearchLockedCandidates(grid, pointers: 2, findPointing: true, out solveInformation)) 
            return true;

        return false;
    }
}