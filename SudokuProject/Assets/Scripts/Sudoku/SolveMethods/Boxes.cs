using System.Collections.Generic;

public static class Boxes
{
    private static TileIndex box1 = new TileIndex(0, 0);
    private static TileIndex box2 = new TileIndex(3, 0);
    private static TileIndex box3 = new TileIndex(6, 0);
    private static TileIndex box4 = new TileIndex(0, 3);
    private static TileIndex box5 = new TileIndex(3, 3);
    private static TileIndex box6 = new TileIndex(6, 3);
    private static TileIndex box7 = new TileIndex(0, 6);
    private static TileIndex box8 = new TileIndex(3, 6);
    private static TileIndex box9 = new TileIndex(6, 6);
    
    public static List<TileIndex> boxes = new List<TileIndex>
    {
        box1, box2, box3,
        box4, box5, box6,
        box7, box8, box9
    };
}