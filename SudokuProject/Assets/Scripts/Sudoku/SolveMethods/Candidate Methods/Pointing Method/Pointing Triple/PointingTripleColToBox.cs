public class PointingTripleColToBox : PointingMethod
{
    public override string GetName => "Pointing Pair Col To Box";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindColToBoxCandidates(grid, 3, out removal);
    }
}