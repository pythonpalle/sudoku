public class PointingPairRowToBox : PointingMethod
{
    public override string GetName => "Pointing Pair Row To Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindRowToBoxCandidates(grid, 2, out removal);
    }
}