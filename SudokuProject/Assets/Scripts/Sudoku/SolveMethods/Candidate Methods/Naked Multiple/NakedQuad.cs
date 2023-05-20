public class NakedQuad : NakedMultiple
{
    public override string GetName => "Naked Quad";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        int multCount = 4;
        return TryFindMultipleInCol(grid, multCount, out removal) 
               || TryFindMultipleInRow(grid, multCount, out removal) 
               || TryFindMultipleInBox(grid, multCount, out removal);
    }
}