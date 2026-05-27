using System.Linq;
using System.Text;
using UnityEngine;

public struct HintText
{
    public string Title;
    public string Description;

    public HintText(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public static HintText Empty => new HintText("", "");
}

public static class HintTextGenerator
{
    public static HintText GenerateHintText(SolutionStepData step, SudokuGrid9x9 grid)
    {
        var method = step.solveMethod;

        if (method is DigitMethod)
        {
            return GetHintTextDigitMethod(step, method);
        }
        
        if (method is CandidateMethod)
        {
            return GetHintTextCandidateMethod(step, grid);
        }

        return HintText.Empty;
    }

    private static HintText GetHintTextCandidateMethod(SolutionStepData step, SudokuGrid9x9 grid)
    {
        var method = step.solveMethod;
        
        string placeHolderTitle =  $"{method.GetName} found";
        string title = placeHolderTitle;
        string description = "";
        
        return new HintText(title, description);
    }

    private static HintText GetHintTextDigitMethod(SolutionStepData step, SolveMethod method)
    {
        TileIndex target = step.tileIndex; // Rutan där siffran ska sättas
        int digit = step.digit; // Siffran som hittades

        string indexString = target.ToAlphaNumeric();

        string placeHolderTitle =  $"{method.GetName} in ({indexString})";
        string title = placeHolderTitle;
        string description = "";
        
        // --- 1. NAKED SINGLE ---
        if (method is NakedSingle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Cell {indexString} has only one remaining candidate digit left.");
            sb.AppendLine();
            sb.AppendLine($"All other digits (1-9) are already blocked by its peers in the same row, column, or box.");
            sb.AppendLine();
            sb.AppendLine($"Therefore, {indexString} must be {digit}.");
            
            description = sb.ToString();
        }

        // --- 2. HIDDEN SINGLE (Box, Row eller Column) ---
        if (method is HiddenSingle)
        {
            var hiddenSingleMethod = (HiddenSingle)method;
            HouseType houseType = hiddenSingleMethod.HouseType;
            string houseName = houseType.ToHouseString(target);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Inside {houseName}, the digit {digit} can only be placed in one single location: {indexString}.");
            sb.AppendLine();
            sb.AppendLine($"Even though cell {indexString} has multiple candidates, no other empty cell in this {houseName} can contain a {digit}.");
            sb.AppendLine();
            sb.AppendLine($"Therefore, {indexString} must be {digit}.");
            
            description = sb.ToString();
        }

        return new HintText(title, description);
    }
}
