using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFish : FishMethod
{
    public override string GetName => "SwordFish";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFish(grid, 3, out solveInformation);
    }
}

public class SwordFishRow : FishMethod
{
    public override string GetName => "SwordFish Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInRow(grid, 3, out solveInformation);
    }
}

public class SwordFishCol : FishMethod
{
    public override string GetName => "SwordFish Col";
    
    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInCol(grid, 3, out solveInformation);
    }
}