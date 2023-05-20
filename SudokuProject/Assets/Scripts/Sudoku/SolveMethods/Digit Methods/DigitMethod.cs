

public abstract class DigitMethod : SolveMethod
{
    // public abstract string GetName { get; }
    // public abstract PuzzleDifficulty Difficulty { get; }
    
    public virtual bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = -1;
        return false;
    }

}