using System.Collections.Generic;
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
        var solveInfo = step.CandidateSolveInformation;
        
        string placeHolderTitle =  $"{method.GetName} found";
        string title = placeHolderTitle;
        string description = "";
        
        // Skapa snygga strängar för rutorna (t.ex. "A1, A2")
        string pointerCellsStr = solveInfo.triggerIndexes.ToAlphaNumeric();
        string removalCellsStr = solveInfo.removalIndexes.ToAlphaNumeric();
        
        
    // --- POINTING METHODS ---
    if (method is PointingMethod)
    {
        int digit = solveInfo.candidateSet.First();
        
        // Hämta källcellerna (Pointers) och de påverkade cellerna
        List<TileIndex> pointers = solveInfo.triggerIndexes;
        List<TileIndex> removals = solveInfo.removalIndexes;



        // Räkna ut hus-namnen geometriskt baserat på källcellerna
        // Eftersom det är Pointing, ligger alla pointers i samma Box
        string sourceBoxName = HouseType.Box.ToHouseString(pointers[0]);
        
        // Ta reda på om de pekar längs en rad eller en kolumn
        bool isRow = pointers.All(p => p.row == pointers[0].row);
        string lineName = isRow 
            ? HouseType.Row.ToHouseString(pointers[0]) 
            : HouseType.Column.ToHouseString(pointers[0]);

        // Justera titeln dynamiskt så den blir mer beskrivande
        title = $"{method.GetName} ({digit}) in {sourceBoxName}";
        
        string pluralString = pointers.Count == 2 ? "both" : "all";
        
        string lineType = isRow ? "row" : "column";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"A pointing pattern for digit {digit} was spotted.");
        sb.AppendLine();
        sb.AppendLine($"Inside {sourceBoxName}, the only remaining cells that can contain a {digit} are {pointerCellsStr}.");
        sb.AppendLine();
        sb.AppendLine($"Since {pluralString} these candidate cells are locked in a straight line along {lineName}, the digit {digit} in that {lineType} MUST be placed in one of them.");
        sb.AppendLine();
        sb.AppendLine($"Consequently, {digit} cannot appear anywhere else in {lineName} outside of {sourceBoxName}.");
        sb.AppendLine();
        sb.AppendLine($"Therefore, we can safely eliminate {digit} from {removalCellsStr}.");

        description = sb.ToString();
    }
    
        // --- CLAIMING METHODS (Line -> Box) ---
    else if (method is ClaimingMethod) // Antar att dina Claiming-klasser ärver från denna
    {
        int digit = solveInfo.candidateSet.First();
        List<TileIndex> pointers = solveInfo.triggerIndexes;

        // Vid Claiming ligger källcellerna på en linje och pekar IN i en box
        bool isRow = pointers.All(p => p.row == pointers[0].row);
        string sourceLineName = isRow ? HouseType.Row.ToHouseString(pointers[0]) : HouseType.Column.ToHouseString(pointers[0]);
        string targetBoxName = HouseType.Box.ToHouseString(pointers[0]);
        string pluralString = pointers.Count == 2 ? "both" : "all";

        title = $"{method.GetName} ({digit}) in {sourceLineName}";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"A claiming pattern (Box-Line Reduction) for digit {digit} was spotted.");
        sb.AppendLine();
        sb.AppendLine($"Inside {sourceLineName}, the only cells that can contain a {digit} are located within {targetBoxName} ({pointerCellsStr}).");
        sb.AppendLine();
        sb.AppendLine($"Since {pluralString} these candidates claim the digit for this line, the {digit} for {sourceLineName} MUST be placed in one of these cells.");
        sb.AppendLine();
        sb.AppendLine($"As a result, no other cell inside {targetBoxName} can claim the {digit}.");
        sb.AppendLine();
        sb.AppendLine($"Therefore, we can safely eliminate {digit} from {removalCellsStr}.");

        description = sb.ToString();
    }

    // --- NAKED MULTIPLES (Naked Pairs / Triples / Quads) ---
    else if (method is NakedMultiple)
    {
        // En lista på siffrorna det gäller, snyggt formaterad via din ToNaturalLanguageList-motor
        string digitsStr = solveInfo.candidateSet.ToNaturalLanguage(); 
        string houseName = solveInfo.houseType.Value.ToHouseString(solveInfo.triggerIndexes[0]);
        string patternName = solveInfo.triggerIndexes.Count switch { 2 => "Pair", 3 => "Triple", _ => "Quad" };

        title = $"Naked {patternName} ({digitsStr}) in {houseName}";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"A Naked {patternName} was spotted in {houseName}.");
        sb.AppendLine();
        sb.AppendLine($"The cells {pointerCellsStr} are locked together because they contain a combined total of exactly {solveInfo.triggerIndexes.Count} candidates: {digitsStr}.");
        sb.AppendLine();
        sb.AppendLine($"This means those {solveInfo.triggerIndexes.Count} digits are strictly reserved for those specific cells. No other values can go there, and those digits cannot go anywhere else in this {houseName}.");
        sb.AppendLine();
        sb.AppendLine($"Therefore, we can safely eliminate {digitsStr} from the other cells in {houseName}: {removalCellsStr}.");

        description = sb.ToString();
    }

    // --- HIDDEN MULTIPLES (Hidden Pairs / Triples / Quads) ---
    else if (method is HiddenMultiple)
    {
        // För att hitta de dolda siffrorna tittar vi på vilka kandidater i trigger-cellerna som INTE tas bort
        var firstTrigger = solveInfo.triggerIndexes[0];
        var hiddenDigits = grid[firstTrigger].Candidates.Where(c => !solveInfo.candidateSet.Contains(c)).ToList();
        string hiddenDigitsStr = hiddenDigits.ToNaturalLanguage();

        string houseName = solveInfo.houseType.Value.ToHouseString(solveInfo.triggerIndexes[0]);
        string patternName = solveInfo.triggerIndexes.Count switch { 2 => "Pair", 3 => "Triple", _ => "Quad" };

        title = $"Hidden {patternName} ({hiddenDigitsStr}) in {houseName}";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"A Hidden {patternName} was spotted in {houseName}.");
        sb.AppendLine();
        sb.AppendLine($"Inside this {houseName}, the digits {hiddenDigitsStr} can only appear within the cells {pointerCellsStr}.");
        sb.AppendLine();
        sb.AppendLine($"Because these {solveInfo.triggerIndexes.Count} specific digits are completely restricted to these {solveInfo.triggerIndexes.Count} cells, no other numbers can occupy them.");
        sb.AppendLine();
        sb.AppendLine($"Therefore, all other \"noise\" candidates ({solveInfo.candidateSet.ToNaturalLanguage()}) can be safely eliminated from {removalCellsStr}, leaving the hidden pattern isolated.");

        description = sb.ToString();
    }
        
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
