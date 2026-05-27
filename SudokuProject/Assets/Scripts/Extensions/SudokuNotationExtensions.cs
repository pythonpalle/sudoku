using System;
using System.Collections.Generic;
using System.Linq;

public static class SudokuNotationExtensions
{
    private static Dictionary<int, char> letterSet = new Dictionary<int, char>()
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
    
    public static string ToAlphaNumeric(this IEnumerable<TileIndex> indexes)
    {
        return ToNaturalLanguageList(indexes, p  => p.ToAlphaNumeric());    
    }
    
    public static string ToNaturalLanguage<T>(this IEnumerable<T> list)
    {
        return ToNaturalLanguageList(list);    
    }
    
    private static string ToNaturalLanguageList<T>(
        IEnumerable<T> items,
        Func<T, string>? selector = null)
    {
        selector ??= x => x?.ToString() ?? "";

        var list = items.Select(selector).ToList();

        return list.Count switch
        {
            0 => "",
            1 => list[0],
            2 => $"{list[0]} & {list[1]}",
            _ => $"{string.Join(", ", list.Take(list.Count - 1))} & {list.Last()}"
        };
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