

using System.Collections.Generic;
using UnityEngine;

public class PointingPair : CandidateMethod
{
    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
    {
        List<TileIndex> indices = new List<TileIndex>();
        removal = new CandidateRemoval();

        if (TryFindPairInRow(grid, out TileIndex rowIndex1, out TileIndex rowIndex2, out int rowDigit))
        {
            indices = GetEffectedRowTiles(grid, rowIndex1, rowDigit);
            if (indices.Count > 0)
            {
                removal.candidate = rowDigit;
                removal.indexes = indices;
                return true;
            }
        }
        
        else if (TryFindPairInCol(grid, out TileIndex colIndex1, out TileIndex colIndex2, out int colDigit))
        {
            indices = GetEffectedColTiles(grid, colIndex1, colDigit);
            if (indices.Count > 0)
            {
                removal.candidate = colDigit;
                removal.indexes = indices;
                return true;
            }
        }
        
        
        Debug.Log("Nothing found with " + GetName);
        return false;
    }

    private List<TileIndex> GetEffectedColTiles(SudokuGrid9x9 grid, TileIndex tileIndex, int digit)
    {
        return GetEffectedTiles(grid, false, tileIndex, digit);
    }

    private List<TileIndex> GetEffectedRowTiles(SudokuGrid9x9 grid, TileIndex tileIndex, int digit)
    {
        return GetEffectedTiles(grid, true, tileIndex, digit);
    }

    private List<TileIndex> GetEffectedTiles(SudokuGrid9x9 grid, bool rowPair, TileIndex tileIndex, int digit)
    {
        int tileRow = tileIndex.row;
        int tileCol = tileIndex.col;

        List<TileIndex> effectedTiles = new List<TileIndex>();
        
        // Tiles in same row or column
        for (int i = 0; i < 9; i++)
        {
            if (rowPair)
            {
                var rowTile = grid[i, tileCol];
                if (rowTile.Candidates.Contains(digit) && rowTile.index != tileIndex)
                    effectedTiles.Add(rowTile.index);
            }
            else
            {
                var colTile = grid[tileRow, i];

                if (colTile.Candidates.Contains(digit) && colTile.index != tileIndex) 
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
                
                if (boxTile.Candidates.Contains(digit) && boxTile.index != tileIndex)
                    effectedTiles.Add(boxTile.index);
            } 
        }

        return effectedTiles;
    }

    private bool TryFindPairInRow(SudokuGrid9x9 grid, out TileIndex tileIndex, out TileIndex tileIndex1, out int digit)
    {
        return TryFindPairIn(grid, true, out tileIndex, out tileIndex1, out digit);
    }
    
    private bool TryFindPairInCol(SudokuGrid9x9 grid, out TileIndex tileIndex, out TileIndex tileIndex1, out int digit)
    {
        return TryFindPairIn(grid, false, out tileIndex, out tileIndex1, out digit);
    }

    private bool TryFindPairIn(SudokuGrid9x9 grid, bool inRow, out TileIndex tileIndex, out TileIndex tileIndex1,
        out int digit)
    {
        List<TileIndex> indices = new List<TileIndex>();
        
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

                        if (!boxTile.Used && boxTile.Candidates.Contains(candidate))
                        {
                            indices.Add(boxTile.index);
                        }
                    } 
                }

                if (indices.Count == 2)
                {
                    if (inRow && indices[0].row == indices[1].row)
                    {
                        tileIndex1 = indices[0];
                        tileIndex = indices[1];
                        digit = candidate;
                        return true;
                    }
                    
                    if (!inRow && indices[0].col == indices[1].col)
                    {
                        tileIndex1 = indices[0];
                        tileIndex = indices[1];
                        digit = candidate;
                        return true;
                    }
                }
            }
        }

        tileIndex1 = tileIndex = new TileIndex();
        digit = -1;
        return false;
    }

    

    public override string GetName => "Pointing Pair";
}