using System.Linq;
using UnityEngine;

public class NakedSingle : DigitMethod
{
    public override bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Candidates.Count == 1)
            {
                index = tile.index;
                digit = tile.Candidates.Min();
                
                return true;
            }
        }

        index = new TileIndex();
        digit = -1;
        return false;
    }
    
    public override string GetName => "Naked Single";

}