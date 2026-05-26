using System.Linq;
using System.Text;
using UnityEngine;

public static class HintTextGenerator
{
    public static string GenerateHintText(SolutionStepData step, SudokuGrid9x9 grid)
    {
        var method = step.solveMethod;

        if (method is DigitMethod)
        {
            return GetHintTextDigitMethod(step, method);
        }
        

        // --- FRAMTIDA METODER (Locked, Naked Multiples, Wings...) ---
        if (method is CandidateMethod)
        {
            var solveInfo = step.CandidateSolveInformation;
            
            if (method is ExtendedWing)
            {
                // Här lägger vi till din XY/XYZ-Wing textmall i nästa steg!
                return "Extended Wing explanation coming soon...";
            }
            
            return $"Look closely at the grid for a {method.GetName}.";
        }

        return $"Look closely at the grid for a {method.GetName}.";
    }

    private static string GetHintTextDigitMethod(SolutionStepData step, SolveMethod method)
    {
        TileIndex target = step.tileIndex; // Rutan där siffran ska sättas
        int digit = step.digit; // Siffran som hittades

        string indexString = target.ToAlphaNumeric();
        
        // --- 1. NAKED SINGLE ---
        if (method is NakedSingle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Naked Single in cell {indexString}:\n");
            sb.AppendLine($"{indexString} has only one remaining candidate digit left.");
            sb.AppendLine($"All other digits (1-9) are already blocked by its peers in the same row, column, or box.");
            sb.AppendLine($"Therefore, {indexString} must be {digit}.");
            return sb.ToString();
        }

        // --- 2. HIDDEN SINGLE (Box, Row eller Column) ---
        if (method is HiddenSingle)
        {
            var hiddenSingleMethod = (HiddenSingle)method;
            HouseType houseType = hiddenSingleMethod.HouseType;
            
            string houseName = houseType.ToHouseString(target);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Hidden Single in {houseName}:\n");
            sb.AppendLine($"Inside {houseName}, the digit {digit} can only be placed in one single location: {indexString}.");
            sb.AppendLine($"Even though cell {indexString} has multiple candidates, no other empty cell in this {houseName} can contain a {digit}.");
            sb.AppendLine($"Therefore, {indexString} must be {digit}.");
            return sb.ToString();
        }

        Debug.LogError($"Method {method} is not supported!");
        return "";
    }
}
