public class HiddenSingleInRow : DigitMethod
{
    public override string GetName => "Hidden Single Row";

    public override bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = 0;

        // check rows
        for (int row = 0; row < 9; row++)
        {
            for (int candidate = 1; candidate <= 9; candidate++)
            {
                int numberOfCandidates = 0;
                
                for (int col = 0; col < 9; col++)
                {
                    var tile = grid[row, col];

                    if (!tile.Used && tile.Candidates.Contains(candidate))
                    {
                        numberOfCandidates++;
                        if (numberOfCandidates > 1)
                        {
                            break;
                        }

                        index = tile.index;
                        digit = candidate;
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
}