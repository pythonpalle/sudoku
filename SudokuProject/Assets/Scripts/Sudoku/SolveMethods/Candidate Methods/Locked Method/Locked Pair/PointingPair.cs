public class PointingPair : LockedPairs
{
    public override string GetName => "Pointing Pair";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        if (SearchLockedCandidates(grid, pointers: 2, findPointing: true, out removal)) 
            return true;

        return false;
        
        //return TryFindBoxToColCandidates(grid, 2, out removal);
    }
}