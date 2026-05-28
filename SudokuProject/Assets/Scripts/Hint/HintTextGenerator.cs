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
        string pointerCellsStr = solveInfo.triggerIndexes.OrderBy(x => x.ToAlphaNumeric()).ToAlphaNumeric();
        string removalCellsStr = solveInfo.removalIndexes.OrderBy(x => x.ToAlphaNumeric()).ToAlphaNumeric();

        string candidateSetString = solveInfo.candidateSet.OrderBy(x => x).ToNaturalLanguage();
        
        
        // --- POINTING METHODS ---
        if (method is PointingMethod)
        {
            int digit = solveInfo.candidateSet.First();
            
            // Hämta källcellerna (Pointers) och de påverkade cellerna
            List<TileIndex> pointers = solveInfo.triggerIndexes;

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
            string houseName = solveInfo.houseType.Value.ToHouseString(solveInfo.triggerIndexes[0]);
            string patternName = solveInfo.triggerIndexes.Count switch { 2 => "Pair", 3 => "Triple", _ => "Quad" };

            title = $"Naked {patternName} ({candidateSetString}) in {houseName}";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"A Naked {patternName} was spotted in {houseName}.");
            sb.AppendLine();
            sb.AppendLine($"The cells {pointerCellsStr} are locked together because they contain a combined total of exactly {solveInfo.triggerIndexes.Count} candidates: {candidateSetString}.");
            sb.AppendLine();
            sb.AppendLine($"This means those {solveInfo.triggerIndexes.Count} digits are strictly reserved for those specific cells. No other values can go there, and those digits cannot go anywhere else in this {houseName}.");
            sb.AppendLine();
            sb.AppendLine($"Therefore, we can safely eliminate {candidateSetString} from the other cells in {houseName}: {removalCellsStr}.");

            description = sb.ToString();
        }

        // --- HIDDEN MULTIPLES (Hidden Pairs / Triples / Quads) ---
        else if (method is HiddenMultiple)
        {
            // För att hitta de dolda siffrorna tittar vi på vilka kandidater i trigger-cellerna som INTE tas bort
            var hiddenDigits = solveInfo.triggerIndexes.SelectMany(x => grid[x].Candidates)
                .Where(c => !solveInfo.candidateSet.Contains(c))
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            
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
            sb.AppendLine($"Therefore, all other \"noise\" candidates ({candidateSetString}) can be safely eliminated from {removalCellsStr}, leaving the hidden pattern isolated.");

            description = sb.ToString();
        }
        
        // --- FISH METHODS (X-Wing / Swordfish / Jellyfish) ---
        else if (method is FishMethod)
        {
            int digit = solveInfo.candidateSet.First();
            List<TileIndex> triggers = solveInfo.triggerIndexes;

            // Ta reda på vilka rader och kolumner som utgör fiskens axlar
            var fishRows = triggers.Select(t => t.row).Distinct().OrderBy(r => r).ToList();
            var fishCols = triggers.Select(t => t.col).Distinct().OrderBy(c => c).ToList();

            // Om basen är Rader (isRowBase) är raderna Base Lines och kolumnerna Cover Lines
            bool isRowBase = solveInfo.houseType == HouseType.Row;
            
            List<int> lineNumbers = isRowBase ? fishRows : fishCols;

            string baseLineNamePlural = isRowBase ? "rows" : "columns";
            string coverLineNamePlural = isRowBase ? "columns" : "rows";

            // rows/cols are 0 index based, need to add 1 to get range (1-9)
            string baseLinesStr = $"{baseLineNamePlural} {lineNumbers.Select(x => x + 1).OrderBy(x => x).ToNaturalLanguage()}";

            // Justera titeln så den blir helt komplett (t.ex. "X-Wing (4) found")
            title = $"{method.GetName} ({digit}) found";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"A {method.GetName} pattern for digit {digit} was spotted.");
            sb.AppendLine();
            sb.AppendLine($"Looking at {baseLinesStr}, the candidate {digit} is strictly confined to exactly {fishRows.Count} intersecting {coverLineNamePlural}.");
            sb.AppendLine();
            sb.AppendLine($"These specific intersection corners ({pointerCellsStr}) form a locked fish grid. Because the digit {digit} must be placed exactly once in each of the {baseLineNamePlural}, it will fully occupy those positions along the {coverLineNamePlural}.");
            sb.AppendLine();
            sb.AppendLine($"Consequently, {digit} cannot be placed anywhere else in those {coverLineNamePlural} outside of the fish pattern.");
            sb.AppendLine();
            sb.AppendLine($"Therefore, we can safely eliminate {digit} from {removalCellsStr}.");

            description = sb.ToString();
        }
        
        // --- UNIQUE RECTANGLE (Type 1) ---
        else if (method is UniquenessRectangle)
        {
            // De två kandidaterna som bildar det dödliga mönstret (t.ex. "2 & 5")
            string deadlyCandidatesStr = candidateSetString;
        
            TileIndex targetCell = solveInfo.removalIndexes.First();
            string targetCellStr = targetCell.ToAlphaNumeric();

            title = $"{method.GetName} found";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"A Unique Rectangle pattern was spotted for candidates {deadlyCandidatesStr}.");
            sb.AppendLine();
            sb.AppendLine($"The cells {pointerCellsStr} form three corners of a rectangle spanning over two rows, two columns, and two boxes. All three of these cells contain ONLY the candidates {deadlyCandidatesStr}.");
            sb.AppendLine();
            sb.AppendLine($"If the fourth corner ({targetCellStr}) also contains only {deadlyCandidatesStr}, it would create a \"Deadly Pattern\" where the puzzle would have two interchangeable, non-unique solutions.");
            sb.AppendLine();
            sb.AppendLine($"Since a valid Sudoku puzzle must have exactly one unique solution, this deadly loop cannot be allowed to form.");
            sb.AppendLine();
            sb.AppendLine($"Therefore, to avoid ambiguity, the candidates {deadlyCandidatesStr} can be safely eliminated from the fourth corner: {targetCellStr}.");

            description = sb.ToString();
        }
        
            // --- EXTENDED WING (XY-Wing / XYZ-Wing) ---
        else if (method is ExtendedWing extendedWing)
        {
            bool isXyz = extendedWing.WingType == ExtendedWingType.XYZ;

            TileIndex pivot = solveInfo.triggerIndexes[0];
            TileIndex wing1 = solveInfo.triggerIndexes[1];
            TileIndex wing2 = solveInfo.triggerIndexes[2];

            string pivotStr = pivot.ToAlphaNumeric();
            string wing1Str = wing1.ToAlphaNumeric();
            string wing2Str = wing2.ToAlphaNumeric();

            // Målsiffran Z (den som ska rensas bort på brädet)
            int zDigit = solveInfo.candidateSet.First();

            // Lista ut X och Y genom att titta på vad Pivot delar med respektive vinge
            var pivotCandidates = grid[pivot].Candidates;
            var wing1Candidates = grid[wing1].Candidates;
            var wing2Candidates = grid[wing2].Candidates;

            // X är siffran som finns i både Pivot och Wing 1 (men som inte är Z)
            int xDigit = pivotCandidates.Intersect(wing1Candidates).FirstOrDefault(c => c != zDigit);
            // Y är siffran som finns i både Pivot och Wing 2 (men som inte är Z)
            int yDigit = pivotCandidates.Intersect(wing2Candidates).FirstOrDefault(c => c != zDigit);

            // Skydd ifall något saknas (t.ex. om man testar i editor med trasig data)
            if (xDigit == 0 || yDigit == 0)
            {
                xDigit = pivotCandidates.First();
                yDigit = pivotCandidates.Last();
            }

            title = $"{method.GetName} found";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"An {method.GetName} pattern was spotted.");
            sb.AppendLine();
            sb.AppendLine($"Pivot Cell: {pivotStr}");
            sb.AppendLine($"Wing 1: {wing1Str}");
            sb.AppendLine($"Wing 2: {wing2Str}");
            sb.AppendLine();

            if (isXyz)
            {
                // XYZ-Wing förklaring
                sb.AppendLine($"The Pivot cell {pivotStr} contains three candidates: {xDigit}, {yDigit} & {zDigit}.");
                sb.AppendLine();
                sb.AppendLine($"If {pivotStr} is {zDigit}, then the target is solved directly.");
                sb.AppendLine($"If {pivotStr} is {xDigit}, then Wing 1 ({wing1Str}) is forced to be {zDigit}.");
                sb.AppendLine($"If {pivotStr} is {yDigit}, then Wing 2 ({wing2Str}) is forced to be {zDigit}.");
                sb.AppendLine();
                sb.AppendLine($"In all three possible scenarios, the digit {zDigit} MUST be placed in either the Pivot, Wing 1, or Wing 2.");
                sb.AppendLine($"Therefore, any cell that \"sees\" all three of these cells simultaneously cannot contain a {zDigit}.");
            }
            else
            {
                // XY-Wing förklaring
                sb.AppendLine($"The Pivot cell {pivotStr} is locked to two candidates: {xDigit} & {yDigit}.");
                sb.AppendLine();
                sb.AppendLine($"If {pivotStr} turns out to be {xDigit}, then Wing 1 ({wing1Str}) is forced to be {zDigit}.");
                sb.AppendLine($"If {pivotStr} turns out to be {yDigit}, then Wing 2 ({wing2Str}) is forced to be {zDigit}.");
                sb.AppendLine();
                sb.AppendLine($"In either case, the digit {zDigit} MUST be placed in either Wing 1 or Wing 2.");
                sb.AppendLine($"Therefore, any cell that \"sees\" both of these wings at the same time cannot contain a {zDigit}.");
            }

            sb.AppendLine();
            sb.AppendLine($"Consequently, we can safely eliminate {zDigit} from {removalCellsStr}.");

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
