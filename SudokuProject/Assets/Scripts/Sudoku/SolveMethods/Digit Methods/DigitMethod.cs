
public abstract class DigitMethod : SolveMethod
{
    
    public virtual bool TryFindDigit(SudokuGrid9x9 grid, out TileIndex index, out int digit)
    {
        index = new TileIndex();
        digit = -1;
        return false;
    }
    
    public override string GetSolveDescription(SolutionStepData solutionStepData)
    {
        return $"{GetName} found in cell {solutionStepData.tileIndex.ToAlphaNumeric()}, revealing a {solutionStepData.digit}.";
    }
}

public abstract class HiddenSingle : DigitMethod
{
    public abstract HouseType HouseType { get; }

    public override string GetName => "Hidden Single";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Easy;
}

public enum HouseType
{
    Row,
    Column,
    Box
}
