using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class MarkClass
{
    private static int maximumMarksForDefaultSize = 5;

    private static string DOUBLE_LINE = "\n\n\n";
    private static string TWO_CHAR_SPACE = "           ";
    private static string THREE_CHAR_SPACE = "        ";
    private static string FOUR_CHAR_SPACE = "     ";
    
    public static void UpdateCenterString(List<int> centerMarks, float defaultSize, TextMeshProUGUI centerText)
    {
        centerMarks.Sort();

        int centerMarkCount = centerMarks.Count;
        float centerStringSize = defaultSize;

        // decreasing font size to make sure all numbers fit in size
        if (centerMarkCount > maximumMarksForDefaultSize)
        {
            int difference = centerMarkCount - maximumMarksForDefaultSize;
            float powBase = 0.87f;

            centerStringSize *= Mathf.Pow(powBase, difference);
        }

        string centerMarkString = string.Empty;
        foreach (var mark in centerMarks)
        {
            centerMarkString += mark.ToString();
        }

        centerText.text = centerMarkString;
        centerText.fontSize = centerStringSize;
    }
    
    public static List<int> GetCornerIndexOrder(int numbersInCorner)
    {
        List<int> indexes = new List<int>();

        // indexes have different positions depending on how many numbers are in corners
        switch (numbersInCorner)
        {
            case < 5:
                for (int i = 0; i < numbersInCorner; i++)
                    indexes.Add(i);
                break;
            
            case 5:
                indexes = new List<int> { 0, 4, 1, 2, 3 };
                break;
            
            case 6:
                indexes = new List<int> { 0, 4, 5, 1, 2, 3 };
                break;
            
            case 7:
                indexes = new List<int> { 0, 4, 5, 1, 2, 6, 3 };
                break;
            
            case 8:
                indexes = new List<int> { 0, 4, 5, 1, 2, 6, 7, 3 };
                break;
            
            case 9:
                indexes = new List<int> { 0, 4, 8, 5, 1, 2, 6, 7, 3 };
                break;
        }

        return indexes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cMs">Corner Marks</param>
    /// <param name="cornerCount"></param>
    /// <returns></returns>
    public static string GetCornersAsString(List<int> cMs)
    {
        switch (cMs.Count)
        {
            case 1:
                return $"{cMs[0]}";
            
            case 2:
                return $"{cMs[0]}" + TWO_CHAR_SPACE +  $"{cMs[1]}";
            
            case 3:
                return $"{cMs[0]}" + TWO_CHAR_SPACE +  $"{cMs[1]}" 
                       + DOUBLE_LINE +
                       $"{cMs[2]}";
            case 4:
                return $"{cMs[0]}" + TWO_CHAR_SPACE +  $"{cMs[1]}" 
                       + DOUBLE_LINE +
                       $"{cMs[2]}" + TWO_CHAR_SPACE +  $"{cMs[3]}" ;

            case 5:
                return $"{cMs[0]}" + " " + $"{cMs[1]}" + THREE_CHAR_SPACE + $"{cMs[2]}"
                       + DOUBLE_LINE +
                       $"{cMs[3]}" + TWO_CHAR_SPACE +  $"{cMs[4]}" ;
            
            case 6:
                return $"{cMs[0]}" + " " + $"{cMs[1]}" + FOUR_CHAR_SPACE + $"{cMs[2]}" + " " + $"{cMs[3]}"
                       + DOUBLE_LINE +
                       $"{cMs[4]}" + TWO_CHAR_SPACE +  $"{cMs[5]}" ;
            
            case 7:
                return $"{cMs[0]}" + " " + $"{cMs[1]}" + FOUR_CHAR_SPACE + $"{cMs[2]}" + " " + $"{cMs[3]}"
                       + DOUBLE_LINE +
                       $"{cMs[4]}" + " " + $"{cMs[5]}" + THREE_CHAR_SPACE +  $"{cMs[6]}" ;
            
            case 8:
                return $"{cMs[0]}" + " " + $"{cMs[1]}" + FOUR_CHAR_SPACE + $"{cMs[2]}" + " " + $"{cMs[3]}"
                       + DOUBLE_LINE +
                       $"{cMs[4]}" + " " + $"{cMs[5]}" + FOUR_CHAR_SPACE +  $"{cMs[6]}" + " " + $"{cMs[7]}" ;
            
            case 9:
                return $"{cMs[0]}" + " " + $"{cMs[1]}" + " " + $"{cMs[2]}" + " " + $"{cMs[3]}" + " " + $"{cMs[4]}"
                       + DOUBLE_LINE +
                       $"{cMs[5]}" + " " + $"{cMs[6]}" + FOUR_CHAR_SPACE +  $"{cMs[7]}" + " " + $"{cMs[8]}" ;
        }
        
        return "";
    }
}