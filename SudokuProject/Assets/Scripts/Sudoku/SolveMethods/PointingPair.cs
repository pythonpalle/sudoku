

using System.Collections.Generic;
using UnityEngine;

public class PointingPair : CandidateMethod
{
    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateRemoval removal)
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

                        if (!boxTile.Used && boxTile.Candidates.Contains(candidate))
                        {
                            indices.Add(boxTile.index);
                        }
                    } 
                }

                if (indices.Count == 2)
                {
                    removal.candidate = candidate;
                    removal.indexes = new List<TileIndex>(2);

                    if (indices[0].row == indices[1].row)
                    {
                        int effectedRow = indices[0].row;
                        
                        for (int i = 0; i < 9; i++)
                        {
                            if (i == indices[0].col || i == indices[1].col) continue;
                            
                            removal.indexes.Add(new TileIndex(effectedRow, i));
                        }

                        Debug.LogWarning("PAIR FOUND!");
                        return true;
                    }
                    
                    else if (indices[0].col == indices[1].col)
                    {
                        int effectedCol = indices[0].col;
                        
                        for (int i = 0; i < 9; i++)
                        {
                            if (i == indices[0].row || i == indices[1].row) continue;
                            
                            removal.indexes.Add(new TileIndex(i, effectedCol));
                        }

                        Debug.LogWarning("PAIR FOUND!");
                        return true;
                    }
                }
            }
        }
        
        // foreach box 1-9:
        //      foreach row 1-3:
        //          foreach number 1-9:
        //              check numCount
        //              if numCount == 2
        //                  return true
        
        Debug.Log("Nothing found with " + GetName);
        return false;
    }

    public override string GetName => "Pointing Pair";
}