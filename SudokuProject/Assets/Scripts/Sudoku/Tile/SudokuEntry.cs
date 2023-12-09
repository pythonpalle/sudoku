using System.Collections.Generic;

public class SudokuEntry
{
    public List<TileBehaviour> tiles = new List<TileBehaviour>();
    public EnterType enterType;
    public int number;
    public bool removal;
    public bool colorRemoval;
    
    public SudokuEntry(List<TileBehaviour> tiles, EnterType enterType, int number, bool removal, bool colorRemoval)
    {
        this.tiles = tiles;
        this.enterType = enterType;
        this.number = number;
        this.removal = removal;
        this.colorRemoval = colorRemoval;
    }

    public SudokuEntry(SudokuEntry other)
    {
        if (other == null)
        {
            return;
        }
        
        tiles = new List<TileBehaviour>(other.tiles);
        enterType = other.enterType;
        number = other.number;
        removal = other.removal;
        colorRemoval = other.colorRemoval;
    }
}