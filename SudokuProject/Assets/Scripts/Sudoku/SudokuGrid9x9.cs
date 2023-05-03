using System.Collections.Generic;

public class SudokuGrid9x9 : SudokuGrid
{
    public SudokuGrid9x9() : base(9) { }
    
    protected override void SetupTiles()
    {
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                Tiles[row, col] = new SudokuTile();
            }
        }
    }
}