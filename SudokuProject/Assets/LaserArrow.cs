using System.Collections.Generic;
using UnityEngine;

public class LaserArrow
{
    public TileIndex StartTile { get; set; }
    public TileIndex EndTile { get; set; }

    public static List<LaserArrow> CalculateLaserArrows(TileIndex targetTileIndex, List<TileIndex> minimalLaserTiles, HouseType houseType, SudokuGrid9x9 sudokuGrid)
    {
        switch (houseType)
        {
            case HouseType.Box:
                return CalculateBoxLaserArrows(targetTileIndex, minimalLaserTiles);
            
            case HouseType.Column:
                return CalculateColumnLaserArrows(targetTileIndex, minimalLaserTiles, sudokuGrid);

            case HouseType.Row:
                return CalculateRowLaserArrows(targetTileIndex, minimalLaserTiles, sudokuGrid);
        }

        Debug.LogError("LaserArrow.CalculateLaserArrows failed");
        return null;
    }
    
    private static List<LaserArrow> CalculateBoxLaserArrows(TileIndex targetTileIndex, List<TileIndex> minimalLaserTiles)
    {
        List<LaserArrow> arrows = new List<LaserArrow>();
    
        // Hitta boxens gränser (min/max för rader och kolumner)
        int boxStartRow = targetTileIndex.row - targetTileIndex.row % 3; // t.ex. 0
        int boxEndRow = boxStartRow + 2;                                // t.ex. 2
        int boxStartCol = targetTileIndex.col - targetTileIndex.col % 3; // t.ex. 0
        int boxEndCol = boxStartCol + 2;                                // t.ex. 2
    
        foreach (var laser in minimalLaserTiles)
        {
            TileIndex endPoint = new TileIndex();
    
            // Fall 1: Lasern ligger på samma RAD som boxen skärs av (Horisontell pil)
            if (laser.row >= boxStartRow && laser.row <= boxEndRow)
            {
                // Kommer lasern från höger? (Som på din bild)
                if (laser.col > boxEndCol)
                {
                    endPoint = new TileIndex(laser.row, boxStartCol); // Sluta längst till vänster i boxen
                }
                // Kommer lasern från vänster?
                else if (laser.col < boxStartCol)
                {
                    endPoint = new TileIndex(laser.row, boxEndCol); // Sluta längst till höger i boxen
                }
            }
            // Fall 2: Lasern ligger på samma KOLUMN som boxen skärs av (Vertikal pil)
            else if (laser.col >= boxStartCol && laser.col <= boxEndCol)
            {
                // Kommer lasern från botten? (Som 6:an längst ner till vänster på din bild)
                if (laser.row > boxEndRow)
                {
                    endPoint = new TileIndex(boxStartRow, laser.col); // Sluta längst upp i boxen
                }
                // Kommer lasern från toppen?
                else if (laser.row < boxStartRow)
                {
                    endPoint = new TileIndex(boxEndRow, laser.col); // Sluta längst ner i boxen
                }
            }
    
            arrows.Add(new LaserArrow { StartTile = laser, EndTile = endPoint });
        }
    
        return arrows;
    }
    
    private static List<LaserArrow> CalculateRowLaserArrows(TileIndex targetTileIndex,
        List<TileIndex> minimalLaserTiles, SudokuGrid9x9 sudokuGrid)
    {
        return CalculateLineLaserArrows(targetTileIndex, minimalLaserTiles, sudokuGrid, true);
    }
    
    private static List<LaserArrow> CalculateColumnLaserArrows(TileIndex targetTileIndex,
        List<TileIndex> minimalLaserTiles, SudokuGrid9x9 sudokuGrid)
    {
        return CalculateLineLaserArrows(targetTileIndex, minimalLaserTiles, sudokuGrid, false);
    }
    
    private static List<LaserArrow> CalculateLineLaserArrows(TileIndex targetTileIndex,
        List<TileIndex> minimalLaserTiles, SudokuGrid9x9 sudokuGrid, bool isRow)
    {
        List<LaserArrow> arrows = new List<LaserArrow>();
    
        // Dynamiskt val av mål-axel baserat på om det är en rad eller kolumn
        int targetLine = isRow ? targetTileIndex.row : targetTileIndex.col;

        foreach (var laser in minimalLaserTiles)
        {
            // Hitta skärningspunkten (Intersection) mellan lasern och mål-linjen
            TileIndex intersection = isRow 
                ? new TileIndex(targetLine, laser.col) 
                : new TileIndex(laser.row, targetLine);
        
            // Kolla om lasern och skärningspunkten delar samma 3x3 box
            if (intersection.GetBox() == laser.GetBox())
            {
                // Hitta boxens start- och slutindex för den axel som rör sig längs linjen
                int currentCoord = isRow ? laser.col : laser.row;
                int startCoord = currentCoord - (currentCoord % 3);
                int endCoord = startCoord + 2;

                for (int coord = startCoord; coord <= endCoord; coord++)
                {
                    TileIndex endPoint = isRow 
                        ? new TileIndex(targetLine, coord) 
                        : new TileIndex(coord, targetLine);
                
                    // Skippa rutor som redan är ifyllda med fasta siffror
                    if (sudokuGrid[endPoint].Used) continue;
                
                    arrows.Add(new LaserArrow { StartTile = laser, EndTile = endPoint });
                }
            }
            else
            {
                // Om de inte är i samma box dras en enda ren linje till skärningspunkten
                arrows.Add(new LaserArrow { StartTile = laser, EndTile = intersection });
            }
        }
    
        return arrows;
    }

}


