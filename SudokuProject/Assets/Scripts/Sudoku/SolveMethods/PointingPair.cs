

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
            Debug.LogWarning($"POINTING ROW TRY AT {rowIndex1} and {rowIndex2} (digit: {rowDigit}");
        
            indices = GetEffectedRowTiles(grid, rowIndex1, rowIndex2, rowDigit);
            if (indices.Count > 0)
            {
                removal.candidate = rowDigit;
                removal.indexes = indices;
                
                Debug.LogWarning($"POINTING ROW PAIR FOUND AT {rowIndex1} and {rowIndex2} (digit: {rowDigit}");
                Debug.Log("Effected tiles: ");
                foreach (var index in removal.indexes)
                {
                    Debug.Log(index);
                }
                
                return true;
            }
        }
        
        if (TryFindPairInCol(grid, out TileIndex colIndex1, out TileIndex colIndex2, out int colDigit))
        {
            Debug.LogWarning($"POINTING COL TRY AT {colIndex1} and {colIndex2} (digit: {colDigit})");

            indices = GetEffectedColTiles(grid, colIndex1, colIndex2, colDigit);
            if (indices.Count > 0)
            {
                removal.candidate = colDigit;
                removal.indexes = indices;
                
                Debug.LogWarning($"POINTING COL PAIR FOUND AT {colIndex1} and {colIndex2} (digit: {colDigit}");
                Debug.Log("Effected tiles: ");
                foreach (var index in removal.indexes)
                {
                    Debug.Log(index);
                }
                
                return true;
            }
        }
        
        // todo: måste gå igenom alla par, inte bara hitta en och sen ge upp
        // TODO: Pointing pair in box (från rad/kolumn), göra tre separata klasser?
        
        
        Debug.Log("Nothing found with " + GetName);
        return false;
    }

    private List<TileIndex> GetEffectedColTiles(SudokuGrid9x9 grid, TileIndex tileIndex, TileIndex tileIndex2, int digit)
    {
        return GetEffectedTiles(grid, false, tileIndex, tileIndex2, digit);
    }

    private List<TileIndex> GetEffectedRowTiles(SudokuGrid9x9 grid, TileIndex tileIndex, TileIndex tileIndex2, int digit)
    {
        return GetEffectedTiles(grid, true, tileIndex, tileIndex2, digit);
    }

    private List<TileIndex> GetEffectedTiles(SudokuGrid9x9 grid, bool rowPair, TileIndex tileIndex, TileIndex tileIndex2, int digit)
    {
        int tileRow = tileIndex.row;
        int tileCol = tileIndex.col;

        List<TileIndex> effectedTiles = new List<TileIndex>();
        
        if (rowPair)
        {
            // Tiles in row
            for (int col = 0; col < 9; col++)
            {
                var rowTile = grid[tileRow, col];

                if (!ValidTile(rowTile, tileIndex, tileIndex2)) continue;

                if (rowTile.Candidates.Contains(digit))
                    effectedTiles.Add(rowTile.index);
            }
        }
        else
        {
            for (int row = 0; row < 9; row++)
            {
                var colTile = grid[row, tileCol];
                if (!ValidTile(colTile, tileIndex, tileIndex2)) continue;
            
                if (colTile.Candidates.Contains(digit))
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
                if (!ValidTile(boxTile, tileIndex, tileIndex2)) continue;
                
                if (boxTile.Candidates.Contains(digit))
                    effectedTiles.Add(boxTile.index);
            } 
        }

        return effectedTiles;
    }

    private bool ValidTile(SudokuTile compareTile, TileIndex pairIndex, TileIndex pairIndex2)
    {
        return !compareTile.Used && compareTile.index != pairIndex && compareTile.index != pairIndex2;
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

                        if (!indices.Contains(boxTile.index) && !boxTile.Used && boxTile.Candidates.Contains(candidate))
                        {
                            indices.Add(boxTile.index);
                        }
                    } 
                }

                if (indices.Count == 2)
                {
                    if (inRow)
                    {
                        if (indices[0].row == indices[1].row)
                        {
                            tileIndex1 = indices[0];
                            tileIndex = indices[1];
                            digit = candidate;
                            return true;
                        }
                    }
                    else
                    {
                        if (indices[0].col == indices[1].col)
                        {
                            tileIndex1 = indices[0];
                            tileIndex = indices[1];
                            digit = candidate;
                            return true;
                        }
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