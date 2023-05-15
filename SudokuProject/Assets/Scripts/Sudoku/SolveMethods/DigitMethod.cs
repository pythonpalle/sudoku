

public abstract class DigitMethod
{
    public bool FindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = -1;
        return false;
    }
}