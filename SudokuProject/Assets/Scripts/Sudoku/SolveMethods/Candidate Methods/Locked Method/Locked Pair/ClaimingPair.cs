public class ClaimingPair : LockedPairs
{
    public override string GetName => "Claiming Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        if (SearchLockedCandidates(grid, pointers: 2, findPointing: false, out removal)) 
            return true;

        return false;
    }
}