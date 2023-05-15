

using UnityEngine;
using UnityEngine.WSA;

public class NakedSingle : SolveMethod
{
    public override bool TryMakeProgress(SudokuGrid9x9 inGrid)
    {
        grid = new SudokuGrid9x9(inGrid);
        
        if (!TryFindProgressIndex(out TileIndex index))
        {
            return false;
        }
        
        HandleNextSolveStep(index);
        //Debug.Log($"Naked single found at ({index.row}, {index.col})");
        
        return true;
    }

    private bool TryFindProgressIndex(out TileIndex tileIndex)
    {
        tileIndex = new TileIndex(-1, -1);
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == 1)
            {
                tileIndex = tile.index;
                return true;
            }
        }

        return false;
    }
}