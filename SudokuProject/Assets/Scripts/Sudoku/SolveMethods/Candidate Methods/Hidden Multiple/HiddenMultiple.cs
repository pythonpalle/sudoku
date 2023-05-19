
using System.Collections.Generic;
using System.Linq;

public abstract class HiddenMultiple : CandidateMethod
{
    protected bool TryFindMultipleInRow(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, true, out removal);
    }
    
    protected bool TryFindMultipleInCol(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, false, out removal);
    }
    
    protected bool TryFindMultipleInBox(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInBox(grid, multCount, out removal);
    }

    private bool CandidatesFromMultipleInRowCol(SudokuGrid9x9 grid, int multCount, bool fromRow, out CandidateRemoval removal)
    {
        List<SudokuTile> nonUseTiles = new List<SudokuTile>();
        removal = new CandidateRemoval();

        for (int row = 0; row < 9; row++)
        {
            nonUseTiles.Clear();
            
            for (int col = 0; col < 9; col++)
            {
                // invert index if checking column
                var tile = fromRow ? grid[row, col] : grid[col, row];

                if (tile.Used) 
                    continue;
                
                nonUseTiles.Add(tile);
            }

            // try find multiples
            if (TryFindHiddenMultipleFromTiles(grid, nonUseTiles, multCount, out removal))
                return true;

        }
        
        return false;
    }

    private bool TryFindHiddenMultipleFromTiles(SudokuGrid9x9 grid, List<SudokuTile> nonUseTiles, int multCount, out CandidateRemoval removal) 
    {
        removal = new CandidateRemoval();
        
        // cant have n tiles that share n candidates if only n-1 tiles exist
        if (nonUseTiles.Count < multCount)
            return false;
        
        
        // key: candidate,
        // value: tiles with that candidate
        Dictionary<int, List<TileIndex>> candidateCount = new Dictionary<int, List<TileIndex>>();

        // store witch tiles contain each index
        for (int candidate = 1; candidate <= 9; candidate++)
        {
            for (int i = 0; i < nonUseTiles.Count; i++)
            {
                if (nonUseTiles[i].Candidates.Contains(candidate))
                {
                    if (candidateCount.ContainsKey(candidate))
                    {
                        candidateCount[candidate].Add(nonUseTiles[i].index);
                    }
                    else
                    {
                        candidateCount.Add(candidate, new List<TileIndex>{nonUseTiles[i].index});

                        // // remove entry if more then multCount tiles share candidate
                        // if (candidateCount[candidate].Count > multCount)
                        // {
                        //     candidateCount.Remove(candidate);
                        //     break;
                        // }
                        
                        // todo: optimera så att digit tiles från den row/col/box ignoreras på en gång
                    }
                }
            }
            
            // // remove entry if less then multCount tiles share the same candidate
            // if (candidateCount.ContainsKey(candidate) && candidateCount[candidate].Count < multCount)
            // {
            //     candidateCount.Remove(candidate);
            // }
        }
        
        
        var multTiles = new List<int>();
        foreach (var candidatePair in candidateCount)
        {
            if (candidatePair.Value.Count != multCount) continue;
            
            multTiles.Clear();
            
            foreach (var compareCandidatePair in candidateCount)
            {
                // don't compare the same candidate
                if (candidatePair.Key == compareCandidatePair.Key) continue;
                
                if (compareCandidatePair.Value.Count != multCount) continue;

                // exact match, those tiles are the hidden multiple
                if (candidatePair.Value.SequenceEqual(compareCandidatePair.Value))
                {
                    multTiles.Add(compareCandidatePair.Key);
                }
            }
            
            // multCount - 1 since the compare tile hasn't been added yet
            if (multTiles.Count == multCount - 1)
            {
                removal.indexes = candidatePair.Value;
                multTiles.Add(candidatePair.Key);
                var candidateSet = GetEffectedCandidates(grid, multTiles.ToHashSet(), candidatePair.Value);
                removal.candidateSet = candidateSet;
                if (removal.candidateSet.Count > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private HashSet<int> GetEffectedCandidates(SudokuGrid9x9 grid, HashSet<int> multCandidates, List<TileIndex> indices)
    {
        var everyCandidate = new HashSet<int>();
        
        // make set include every candidate from effected indices
        foreach (var index in indices)
        {
            everyCandidate.UnionWith(grid[index].Candidates);
        }
        
        // now remove the candidate multiple (pair, triple, etc)
        everyCandidate.ExceptWith(multCandidates);

        return everyCandidate;
    }

    private bool CandidatesFromMultipleInBox(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        List<SudokuTile> multTiles = new List<SudokuTile>();
        removal = new CandidateRemoval();
        
        foreach (var box in Boxes.boxes)
        {
            multTiles.Clear();

            for (int deltaRow = 0; deltaRow < 3; deltaRow++)
            {
                for (int deltaCol = 0; deltaCol < 3; deltaCol++)
                {
                    var tile = grid[box.row + deltaRow, box.col + deltaCol];
                    
                    if (tile.Used) 
                        continue;
                
                    multTiles.Add(tile);
                }
            }
            
            if (TryFindHiddenMultipleFromTiles(grid,  multTiles, multCount, out removal))
                return true;
        }

        return false;
    }
}