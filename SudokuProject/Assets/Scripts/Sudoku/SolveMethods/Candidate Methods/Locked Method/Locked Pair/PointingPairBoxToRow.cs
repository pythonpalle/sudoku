//
//
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PointingPairBoxToRow : LockedCandidatesMethod
// {
//     public override string GetName => "Pointing Pair Box To Row";
//     public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;
//
//     public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
//     {
//         if (SearchLockedCandidates(grid, pointers: 2, findPointing: true, out removal)) return true;
//
//         return false;
//         //return TryFindBoxToRowCandidates(grid, 2, out removal);
//     }
// }