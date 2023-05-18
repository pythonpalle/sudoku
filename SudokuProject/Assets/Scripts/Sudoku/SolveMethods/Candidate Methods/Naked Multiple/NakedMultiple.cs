
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
        List<SudokuTile> tiles = new List<SudokuTile>();
        removal = new CandidateRemoval();

        for (int row = 0; row < 9; row++)
        {
            tiles.Clear();
            
            for (int col = 0; col < 9; col++)
            {
                // invert index if checking column
                var tile = fromRow ? grid[row, col] : grid[col, row];

                if (tile.Used || tile.Entropy != multCount) 
                    continue;
                
                tiles.Add(tile);
            }

            // try find multiples
            if (!TryFindMultipleFromTiles(tiles, multCount, out var multTiles))
                continue;

            if (TryFindEffectedTilesFromMultRowCol(grid, multTiles, fromRow, out removal))
            {
                return true;
            }
            
        }
        
        return false;
    }

    private bool TryFindEffectedTilesFromMultRowCol(SudokuGrid9x9 grid, List<TileIndex> multTiles, bool fromRow, out CandidateRemoval removal)
    {
        var tileIndex = multTiles[0];
        
        int multRow = tileIndex.row;
        int multCol = tileIndex.col;

        removal = new CandidateRemoval();
        removal.candidateSet = grid[tileIndex].Candidates;

        
        for (int i = 0; i < 9; i++)
        {
            var compareTile = fromRow ? grid[multRow, i] : grid[i, multCol];
            
            if (!ValidTile(compareTile, multTiles))
                continue;
            
            removal.indexes.Add(compareTile.index);
        }

        return false;
    }

    private bool TryFindMultipleFromTiles(List<SudokuTile> tiles, int multCount, out List<TileIndex> outTiles) 
    {
        outTiles = new List<TileIndex>();
        
        // cant have n tiles that share n candidates if only n-1 tiles exist
        if (tiles.Count < multCount)
            return false;
        
        HashSet<int> candidateSet = new HashSet<int>();

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            candidateSet = tile.Candidates;
            outTiles.Clear();
            
            for (int j = i+1; j < tiles.Count; j++)
            {
                var compareTile = tiles[j];
                var compareSet = compareTile.Candidates;

                if (compareSet == candidateSet)
                {
                    outTiles.Add(compareTile.index);
                }
            }

            // multCount - 1 since the compare tile hasn't been added yet
            if (outTiles.Count == multCount - 1)
            {
                outTiles.Add(tile.index);
                return true;
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
        return TryFindMultipleInCol(grid, multCount, out removal) || TryFindMultipleInRow(grid, multCount, out removal);
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