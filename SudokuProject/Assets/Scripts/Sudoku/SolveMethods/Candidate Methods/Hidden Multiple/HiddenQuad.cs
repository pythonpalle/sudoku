public class HiddenQuad : HiddenMultiple
{
    public override string GetName => "Hidden Quad";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInCol(grid, 4, out removal) ||
         TryFindMultipleInRow(grid, 4, out removal) ||
         TryFindMultipleInBox(grid, 4, out removal);
    }
}