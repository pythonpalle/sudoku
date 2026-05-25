using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class FishMethod : CandidateMethod
{
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Extreme;

    // Den centrala sök-motorn för X-Wing (2), Swordfish (3) och Jellyfish (4)
    protected bool SearchFish(SudokuGrid9x9 grid, int fishSize, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // Loopa igenom alla 9 siffror (kandidater) separat
        for (int digit = 1; digit <= 9; digit++)
        {
            // Testa både med rader som bas-linjer och kolumner som bas-linjer
            if (EvaluateFishLines(grid, digit, fishSize, isRowBase: true, out solveInformation)) return true;
            if (EvaluateFishLines(grid, digit, fishSize, isRowBase: false, out solveInformation)) return true;
        }

        return false;
    }

    private bool EvaluateFishLines(SudokuGrid9x9 grid, int digit, int fishSize, bool isRowBase, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // 1. Kartlägg vilka linjer (0-8) som innehåller vår kandidat och var de finns
        // Key: Linje-index, Value: Lista på korsande koordinater där siffran finns
        Dictionary<int, List<int>> lineCoverage = new Dictionary<int, List<int>>();

        for (int i = 0; i < 9; i++)
        {
            List<int> intersections = new List<int>();
            for (int j = 0; j < 9; j++)
            {
                var tile = isRowBase ? grid[i, j] : grid[j, i];
                if (!tile.Used && tile.Candidates.Contains(digit))
                {
                    intersections.Add(j);
                }
            }

            // En bas-linje är bara intressant om den har mellan 2 och 'fishSize' antal placeringar
            if (intersections.Count >= 2 && intersections.Count <= fishSize)
            {
                lineCoverage[i] = intersections;
            }
        }

        // Om vi har färre giltiga linjer än fiskens storlek kan det inte bli en fisk
        if (lineCoverage.Count < fishSize) return false;

        // 2. Generera alla kombinationer av dessa giltiga linjer med storleken 'fishSize'
        List<int> validLines = lineCoverage.Keys.ToList();
        List<List<int>> lineCombinations = new List<List<int>>();
        GenerateCombinations(validLines, fishSize, 0, new List<int>(), lineCombinations);

        // 3. Utvärdera varje kombination av linjer
        foreach (var baseLines in lineCombinations)
        {
            // Samla alla unika korsande linjer (Cover Lines) som dessa bas-linjer har tillsammans
            HashSet<int> coverLines = new HashSet<int>();
            foreach (int line in baseLines)
            {
                coverLines.UnionWith(lineCoverage[line]);
            }

            // GULDREGELN FÖR FISKAR:
            // Det är en giltig fisk OM antalet unika cover-linjer är EXAKT lika med antalet bas-linjer (fishSize)!
            if (coverLines.Count != fishSize) continue;

            // 4. Identifiera Trigger-celler (cellerna som utgör själva fisken)
            List<TileIndex> triggerIndexes = new List<TileIndex>();
            foreach (int baseLine in baseLines)
            {
                foreach (int coverLine in coverLines)
                {
                    TileIndex idx = isRowBase ? new TileIndex(baseLine, coverLine) : new TileIndex(coverLine, baseLine);
                    if (!grid[idx].Used && grid[idx].Candidates.Contains(digit))
                    {
                        triggerIndexes.Add(idx);
                    }
                }
            }

            // 5. Leta efter raderings-celler (Removal Indexes) i cover-linjerna, men UTANFÖR våra bas-linjer
            List<TileIndex> removalIndexes = new List<TileIndex>();

            foreach (int coverLine in coverLines)
            {
                for (int checkLine = 0; checkLine < 9; checkLine++)
                {
                    // Skippa om den aktuella linjen är en av fiskens bas-linjer
                    if (baseLines.Contains(checkLine)) continue;

                    TileIndex checkIdx = isRowBase ? new TileIndex(checkLine, coverLine) : new TileIndex(coverLine, checkLine);
                    if (!grid[checkIdx].Used && grid[checkIdx].Candidates.Contains(digit))
                    {
                        removalIndexes.Add(checkIdx);
                    }
                }
            }

            // Om vi hittade kandidater som faktiskt kan tas bort, har vi en vinnare!
            if (removalIndexes.Count > 0)
            {
                // 6. Skapa det fulla visuella rutnätet (alla celler i de inblandade raderna och kolumnerna)
                // Det här gör att ditt GUI kan rita upp de korsande ljuslinjerna fantastiskt snyggt!
                List<TileIndex> fullVisualGrid = new List<TileIndex>();
                for (int i = 0; i < 9; i++)
                {
                    foreach (int baseLine in baseLines)
                        fullVisualGrid.Add(isRowBase ? new TileIndex(baseLine, i) : new TileIndex(i, baseLine));

                    foreach (int coverLine in coverLines)
                        fullVisualGrid.Add(isRowBase ? new TileIndex(i, coverLine) : new TileIndex(coverLine, i));
                }

                solveInformation = new CandidateSolveInformation(
                    removalIndexes,
                    new HashSet<int> { digit },
                    triggerIndexes,
                    fullVisualGrid.Distinct().ToList(),
                    isRowBase ? HouseType.Row : HouseType.Column
                );

                #if UNITY_EDITOR
                Debug.Log($"[Fish Hittad!] Storlek: {fishSize}, Siffra: {digit}, Bas-linjer: {string.Join(",", baseLines)}, korsade linjer: {string.Join(",", coverLines)}");
                #endif

                return true;
            }
        }

        return false;
    }

    // Enkel rekursiv kombinations-hjälpare (Samma struktur som för Naked/Hidden)
    private void GenerateCombinations(List<int> source, int k, int start, List<int> current, List<List<int>> result)
    {
        if (current.Count == k)
        {
            result.Add(new List<int>(current));
            return;
        }

        for (int i = start; i < source.Count; i++)
        {
            current.Add(source[i]);
            GenerateCombinations(source, k, i + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
}
