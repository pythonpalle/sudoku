using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : FishMethod
{
    public override string GetName => "JellyFish";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFish(grid, 4, out solveInformation);
    }
}

public class JellyFishRow : FishMethod
{
    public override string GetName => "JellyFish Row";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInRow(grid, 4, out solveInformation);
    }
}

public class JellyFishCol : FishMethod
{
    public override string GetName => "SwordFish Col";


    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        return TryFindFishInCol(grid, 4, out solveInformation);
    }
}