
using System.Collections.Generic;

public abstract class NakedMultiple : CandidateMethod
{
    protected bool TryFindMultipleInRow(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, true, out removal);
    }
    
    protected bool TryFindMultipleInCol(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromMultipleInRowCol(grid, multCount, false, out removal);
    }

    private bool CandidatesFromMultipleInRowCol(SudokuGrid9x9 grid, int multCount, bool fromRow, out CandidateRemoval removal)
    {
        List<SudokuTile> multTiles = new List<SudokuTile>();
        removal = new CandidateRemoval();

        for (int row = 0; row < 9; row++)
        {
            multTiles.Clear();
            
            for (int col = 0; col < 9; col++)
            {
                // invert index if checking column
                var tile = fromRow ? grid[row, col] : grid[col, row];

                if (tile.Used || tile.Entropy != multCount) 
                    continue;
                
                multTiles.Add(tile);
            }

            // try find multiples
            if (TryFindMultipleFromTiles(grid, fromRow, multTiles, multCount, out removal))
                return true;

            // if (TryFindEffectedTilesFromMultRowCol(grid, multTiles, fromRow, out removal))
            // {
            //     return true;
            // }

        }
        
        return false;
    }

    private bool TryFindEffectedTilesFromMultRowCol(SudokuGrid9x9 grid, List<TileIndex> multTiles, bool fromRow, out CandidateRemoval removal)
    {
        // todo: testa ej ha som out parameter
        
        
        var tileIndex = multTiles[0];
        
        int multRow = tileIndex.row;
        int multCol = tileIndex.col;

        var candidateSet = grid[tileIndex].Candidates;
        bool foundEffected = false;

        removal = new CandidateRemoval(new List<TileIndex>(), candidateSet);

        for (int i = 0; i < 9; i++)
        {
            var compareTile = fromRow ? grid[multRow, i] : grid[i, multCol];
            
            if (!ValidTile(compareTile, multTiles))
                continue;

            if (candidateSet.Overlaps(compareTile.Candidates))
            {
                removal.indexes.Add(compareTile.index);
                foundEffected = true;
            }
        }

        return foundEffected;
    }

    private bool TryFindMultipleFromTiles(SudokuGrid9x9 grid, bool fromRow, List<SudokuTile> rightEntropyTiles, int multCount, out CandidateRemoval removal) 
    {
        var multTiles = new List<TileIndex>(multCount);
        removal = new CandidateRemoval();
        
        // cant have n tiles that share n candidates if only n-1 tiles exist
        if (rightEntropyTiles.Count < multCount)
            return false;
        
        HashSet<int> candidateSet = new HashSet<int>();

        for (int i = 0; i < rightEntropyTiles.Count; i++)
        {
            multTiles.Clear();

            var tile = rightEntropyTiles[i];
            candidateSet = tile.Candidates;
            
            for (int j = i+1; j < rightEntropyTiles.Count; j++)
            {
                var compareTile = rightEntropyTiles[j];
                var compareSet = compareTile.Candidates;

                if (candidateSet.SetEquals(compareSet))
                {
                    multTiles.Add(compareTile.index);
                }
            }

            // multCount - 1 since the compare tile hasn't been added yet
            if (multTiles.Count == multCount - 1)
            {
                multTiles.Add(tile.index);
                
                if (TryFindEffectedTilesFromMultRowCol(grid, multTiles, fromRow, out removal))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public class NakedPair : NakedMultiple
{
    public override string GetName => "Naked Pair";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        // todo: lägg till box check
        
        int multCount = 2;
        return TryFindMultipleInCol(grid, multCount, out removal); // || TryFindMultipleInRow(grid, multCount, out removal);
    }
}

public class NakedTriple : NakedMultiple
{
    public override string GetName => "Naked Triple";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        // todo: lägg till box check
        
        int multCount = 3;
        return TryFindMultipleInCol(grid, multCount, out removal) || TryFindMultipleInRow(grid, multCount, out removal);
    }
}

public class NakedQuad : NakedMultiple
{
    public override string GetName => "Naked Quad";

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        int multCount = 4;
        return TryFindMultipleInCol(grid, multCount, out removal) || TryFindMultipleInRow(grid, multCount, out removal);
    }
}