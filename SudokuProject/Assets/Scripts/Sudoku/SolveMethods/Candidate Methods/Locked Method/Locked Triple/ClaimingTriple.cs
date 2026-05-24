public class ClaimingTriple : LockedTriples
{
    public override string GetName => "Claiming Triple";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        if (SearchLockedCandidates(grid, pointers: 3, findPointing: false, out removal)) 
            return true;

        return false;
    }
}