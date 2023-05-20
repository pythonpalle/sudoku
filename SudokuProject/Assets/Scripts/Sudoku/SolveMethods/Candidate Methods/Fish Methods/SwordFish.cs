using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFish : FishMethod
{
    public override string GetName => "SwordFish";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFish(grid, 3, out removal);
    }
}

public class SwordFishRow : FishMethod
{
    public override string GetName => "SwordFish Row";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFishInRow(grid, 3, out removal);
    }
}

public class SwordFishCol : FishMethod
{
    public override string GetName => "SwordFish Col";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        return TryFindFishInCol(grid, 3, out removal);
    }
}