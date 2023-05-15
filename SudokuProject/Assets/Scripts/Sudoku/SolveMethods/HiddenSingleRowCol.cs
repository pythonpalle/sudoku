public class HiddenSingleRowCol : DigitMethod
{
    public override string GetName => "Hidden Single Row/Col";

    public override bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = 0;

        // check columns
        for (int row = 1; row <= 9; row++)
        {
            for (int candidate = 0; candidate < 9; candidate++)
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
        
        // check rows
        for (int col = 1; col <= 9; col++)
        {
            for (int candidate = 0; candidate < 9; candidate++)
            {
                int numberOfCandidates = 0;
                
                for (int row = 0; row < 9; row++)
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