public class PointingTriple : LockedTriples
{
    public override string GetName => "Pointing Triple";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        if (SearchLockedCandidates(grid, pointers: 3, findPointing: true, out solveInformation)) 
            return true;

        return false;
    }
}