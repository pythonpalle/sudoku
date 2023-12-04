using System.Collections.Generic;

public class SudokuCommand
{
    public List<TileBehaviour> tiles;
    public int number;
    public EnterType enterType;
    public bool entry;
    public bool colorRemoval;
    
    public SudokuCommand(List<TileBehaviour> tiles, int number, EnterType enterType, bool entry, bool colorRemoval = false)
    {
        this.tiles = tiles;
        this.number = number;
        this.enterType = enterType;
        this.entry = entry;
        this.colorRemoval = colorRemoval;
    }

    public void Execute()
    {
            
    }
    
    public void Undo()
    {
        
    }
}