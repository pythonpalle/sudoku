using System.Collections.Generic;

public class AddDigitCommand : SudokuCommand
{
    private List<int> previousDigits = new List<int>();
    private int number;
    
    public AddDigitCommand(List<TileBehaviour> tiles, int number) : base(tiles)
    {
        previousDigits = new List<int>(tiles.Count);
        FillPreviousDigits();
        this.number = number;
    }

    private void FillPreviousDigits()
    {
        for (int i = 0; i < tiles.Count; i++) 
        {
            previousDigits.Add(tiles[i].number);
        }
    }

    public override void Execute()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TryUpdateNumber(number, EnterType.DigitMark, false);
        }
    }

    public override void Undo()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TryUpdateNumber(previousDigits[i], EnterType.DigitMark, false);
        }
    }
}

public class RemoveDigitCommand : SudokuCommand
{
    private List<int> previousDigits = new List<int>();
    private int number;
    
    public RemoveDigitCommand(List<TileBehaviour> tiles, int number) : base(tiles)
    {
        previousDigits = new List<int>(tiles.Count);
        FillPreviousDigits();
        this.number = number;
    }

    private void FillPreviousDigits()
    {
        for (int i = 0; i < tiles.Count; i++) 
        {
            previousDigits.Add(tiles[i].number);
        }
    }

    public override void Execute()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TryUpdateNumber(number, EnterType.DigitMark, true);
        }
    }

    public override void Undo()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TryUpdateNumber(previousDigits[i], EnterType.DigitMark, false);
        }
    }
}