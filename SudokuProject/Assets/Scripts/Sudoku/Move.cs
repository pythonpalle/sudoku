using System.Collections.Generic;

public struct Move
{
    public SudokuTile Tile;
    public int Number;
    public List<SudokuTile> EffectedTiles;

    public Move(SudokuTile tile, int number, List<SudokuTile> effectedTiles)
    {
        Tile = tile;
        Number = number;
        EffectedTiles = effectedTiles;
    }
}