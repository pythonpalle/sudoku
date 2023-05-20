public class XYZWing : ExtendedWing
{
    public override string GetName => "XYZ-Wing";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindWingCandidates(grid, true, out removal);
    }
}