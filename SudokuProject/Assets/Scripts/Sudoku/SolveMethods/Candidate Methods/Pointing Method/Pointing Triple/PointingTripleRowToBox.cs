public class PointingTripleRowToBox : PointingMethod
{
    public override string GetName => "Pointing Triple Row To Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindRowToBoxCandidates(grid, 3, out removal);
    }
}