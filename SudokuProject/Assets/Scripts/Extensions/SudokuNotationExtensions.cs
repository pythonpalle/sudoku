using System.Collections.Generic;

public static class SudokuNotationExtensions
{
    // // Omvandlar TileIndex(row: 2, col: 4) till "R3C5"
    // public static string ToRCString(this TileIndex index)
    // {
    //     return $"R{index.row + 1}C{index.col + 1}";
    // }

    public static Dictionary<int, char> letterSet = new Dictionary<int, char>()
    {
        { 1, 'A' },
        { 2, 'B' },
        { 3, 'C' },
        { 4, 'D' },
        { 5, 'E' },
        { 6, 'F' },
        { 7, 'G' },
        { 8, 'H' },
        { 9, 'I' },
    };

    public static string ToAlphaNumeric(this TileIndex index)
    {
        return $"{letterSet[index.row + 1]}{index.col + 1}";
    }

    // Ger ett snyggt namn på huset samt dess nummer (1-9)
    public static string ToHouseString(this HouseType type, TileIndex index)
    {
        switch (type)
        {
            case HouseType.Row: 
                return $"Row {index.row + 1}";
            case HouseType.Column: 
                return $"Column {index.col + 1}";
            case HouseType.Box: 
                // Räknar ut boxens nummer (1 till 9) från topp-vänster till botten-höger
                int boxNum = (index.row / 3) * 3 + (index.col / 3) + 1;
                return $"Box {boxNum}";
            default: 
                return "House";
        }
    }
}