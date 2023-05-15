

public abstract class DigitMethod
{
    public virtual bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = -1;
        return false;
    }

    public virtual string GetName => "";
}