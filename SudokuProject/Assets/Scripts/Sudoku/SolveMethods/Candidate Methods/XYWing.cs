﻿

using System;

public class XYWing : ExtendedWing
{
    public override string GetName => "XY-Wing";

    public override PuzzleDifficulty Difficulty
    {
        get => PuzzleDifficulty.Hard;
    }

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindWingCandidates(grid, false, out removal);
    }
}