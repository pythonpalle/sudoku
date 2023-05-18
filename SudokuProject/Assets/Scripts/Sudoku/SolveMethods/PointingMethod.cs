using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PointingMethod : CandidateMethod
{
    protected bool TryFindBoxToRowCandidates(SudokuGrid9x9 grid, int pointers, out CandidateRemoval removal)
    {
        return CandidatesFromBox(grid, pointers, true, out removal);
    }
    
    protected bool TryFindBoxToColCandidates(SudokuGrid9x9 grid, int pointers, out CandidateRemoval removal)
    {
        return CandidatesFromBox(grid, pointers, false, out removal);
    }

    private bool CandidatesFromBox(SudokuGrid9x9 grid, int pointers, bool checkRow, out CandidateRemoval removal)
    {
        List<TileIndex> indices = new List<TileIndex>();
        removal = new CandidateRemoval();

        // for every box
        foreach (var box in Boxes.boxes)
        {
            int row = box.row;
            int col = box.col;
            
            // for every number
            for (int candidate = 1; candidate <= 9; candidate++)
            {
                indices.Clear();
                
                // for every cell in box
                for (int deltaRow = 0; deltaRow < 3; deltaRow++)
                {
                    for (int deltaCol = 0; deltaCol < 3; deltaCol++)
                    {
                        SudokuTile boxTile = grid[row + deltaRow, col + deltaCol];

                        if (!indices.Contains(boxTile.index) && !boxTile.Used && boxTile.Candidates.Contains(candidate))
                        {
                            indices.Add(boxTile.index);
                        }
                    } 
                }

                if (indices.Count == pointers && AllIndicesHaveSame(indices, checkRow))
                {
                    List<TileIndex> effectedTileIndices = FindEffectedIndicesFromBox(grid, indices, pointers, candidate, checkRow);
                    if (effectedTileIndices.Count > 0)
                    {
                        removal.candidate = candidate;
                        removal.indexes = effectedTileIndices;
                        Debug.LogWarning($"Found pointing at {indices[0]}, {indices[1]} (digit: {candidate}");
                        Debug.Log("Effected indices: ");
                        foreach (var index in removal.indexes)
                        {
                            Debug.Log(index);
                        }
                        
                        
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    private List<TileIndex> FindEffectedIndicesFromBox(SudokuGrid9x9 grid, List<TileIndex> indices, int pointers, int candidate, bool checkRow)
    {
        int tileRow = indices[0].row;
        int tileCol = indices[0].col;

        List<TileIndex> effectedTiles = new List<TileIndex>();
        
        if (checkRow)
        {
            // Tiles in row
            for (int col = 0; col < 9; col++)
            {
                var rowTile = grid[tileRow, col];

                if (!ValidTile(rowTile, indices)) continue;

                if (rowTile.Candidates.Contains(candidate))
                    effectedTiles.Add(rowTile.index);
            }
        }
        else
        {
            for (int row = 0; row < 9; row++)
            {
                var colTile = grid[row, tileCol];
                if (!ValidTile(colTile, indices)) continue;
            
                if (colTile.Candidates.Contains(candidate))
                    effectedTiles.Add(colTile.index);
            }
        }

        // Tiles in same box
        int topLeftBoxRow = tileRow - tileRow % 3;
        int topLeftBoxCol = tileCol - tileCol % 3;

        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                SudokuTile boxTile = grid[topLeftBoxRow + deltaRow, topLeftBoxCol + deltaCol];
                if (!ValidTile(boxTile, indices)) continue;
                
                if (boxTile.Candidates.Contains(candidate))
                    effectedTiles.Add(boxTile.index);
            } 
        }

        return effectedTiles;
    }

    private bool ValidTile(SudokuTile compareTile, List<TileIndex> indices)
    {
        if (compareTile.Used) return false;

        foreach (var index in indices)
        {
            if (index == compareTile.index)
                return false;
        }

        return true;

        return indices.All(index => index != compareTile.index);
    }

    private bool AllIndicesHaveSame(List<TileIndex> tileIndices, bool checkRow)
    {

        if (checkRow)
        {
            int tileRow = tileIndices[0].row;
            return tileIndices.All(tile => tile.row == tileRow);
        }
        else
        {
            int tileCol = tileIndices[0].col;
            return tileIndices.All(tile => tile.col == tileCol);
        }
    }
}

