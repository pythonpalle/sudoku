// public class PointingTripleBoxToRow : LockedTriples
// {
//     public override string GetName => "Pointing Triple Box To Row";
//     public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Hard;
//
//     public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
//     {
//         if (SearchLockedCandidates(grid, pointers: 3, findPointing: true, out removal)) return true;
//
//         return false;
//         
//        // return TryFindBoxToRowCandidates(grid, 3, out removal);
//     }
// }