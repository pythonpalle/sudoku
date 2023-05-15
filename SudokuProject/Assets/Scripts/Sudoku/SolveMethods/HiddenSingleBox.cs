

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class HiddenSingleBox : DigitMethod
{
    private static TileIndex box1 = new TileIndex(0, 0);
    private static TileIndex box2 = new TileIndex(3, 0);
    private static TileIndex box3 = new TileIndex(6, 0);
    private static TileIndex box4 = new TileIndex(0, 3);
    private static TileIndex box5 = new TileIndex(3, 3);
    private static TileIndex box6 = new TileIndex(6, 3);
    private static TileIndex box7 = new TileIndex(0, 6);
    private static TileIndex box8 = new TileIndex(3, 6);
    private static TileIndex box9 = new TileIndex(6, 6);

    private static List<TileIndex> boxes = new List<TileIndex>
    {
        box1, box2, box3,
        box4, box5, box6,
        box7, box8, box9
    };
    
    public override bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = 0;

        foreach (var box in boxes)
        {
            int row = box.row;
            int col = box.col;
            
            for (int candidate = 1; candidate <= 9; candidate++)
            {
                bool severalFound = false;
                int numberOfCandidates = 0;
                
                for (int deltaRow = 0; deltaRow < 3; deltaRow++)
                {
                    if (severalFound) break;
                    
                    for (int deltaCol = 0; deltaCol < 3; deltaCol++)
                    {
                        SudokuTile boxTile = grid[row + deltaRow, col + deltaCol];

                        if (!boxTile.Used && boxTile.Candidates.Contains(candidate))
                        {
                            numberOfCandidates++;
                            if (numberOfCandidates > 1)
                            {
                                severalFound = true;
                                break;
                            }

                            index = boxTile.index;
                            digit = candidate;
                        }
                    } 
                }

                if (numberOfCandidates == 1)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public override string GetName => "Hidden Single Box";

}