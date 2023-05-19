﻿public class HiddenPairInRow : HiddenMultiple
{
    public override string GetName => "Hidden Pair In Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindMultipleInRow(grid, 2, out removal);
    }
}