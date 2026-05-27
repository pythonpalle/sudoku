public class ClaimingPair : ClaimingMethod
{
    public override string GetName => "Claiming Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        if (SearchLockedCandidates(grid, pointers: 2, findPointing: false, out solveInformation)) 
            return true;

        return false;
    }
}