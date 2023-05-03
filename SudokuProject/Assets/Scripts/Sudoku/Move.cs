public struct Move
{
    public SudokuTile Tile;
    public int Number;

    public Move(SudokuTile tile, int number)
    {
        Tile = tile;
        Number = number;
    }
}