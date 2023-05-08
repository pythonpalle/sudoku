using System.Collections.Generic;

public struct Move
{
    public TileIndex Index;
    public int Number;
    public List<TileIndex> EffectedTileIndecies;

    public Move(TileIndex index, int number, List<TileIndex> effectedTileIndecies)
    {
        Index = index;
        Number = number;
        EffectedTileIndecies = effectedTileIndecies;
    }
}