using UnityEngine;
using UnityEngine.WSA;

public class HiddenSingleBox : DigitMethod
{
    

   
    
    public override bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = 0;

        foreach (var box in Boxes.boxes)
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