// public class PointingPairRowToBox : LockedCandidatesMethod
// {
//     public override string GetName => "Pointing Pair Row To Box";
//     public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Medium;
//
//
//     public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
//     {
//         if (SearchLockedCandidates(grid, pointers: 2, findPointing: false, out removal)) return true;
//
//         return false;
//       //  return TryFindRowToBoxCandidates(grid, 2, out removal);
//     }
// }