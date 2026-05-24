using System.Collections.Generic;
using System.Linq;

public abstract class HiddenMultiple : CandidateMethod
{
    protected abstract int multCount { get; }
    
    protected bool SearchHiddenMultiples(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        // check box first, easier to find for humans
        if (SearchHiddenMultiples(grid, HouseType.Box, out solveInformation)) return true;

        if (SearchHiddenMultiples(grid, HouseType.Row, out solveInformation)) return true;
        if (SearchHiddenMultiples(grid, HouseType.Column, out solveInformation)) return true;

        return false;
    }
    
    // Den centrala sök-motorn för BÅDE Rader, Kolumner och Boxar
    private bool SearchHiddenMultiples(SudokuGrid9x9 grid, HouseType houseType, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // Loopa igenom de 9 husen (0-8) av den valda typen
        for (int houseIndex = 0; houseIndex < 9; houseIndex++)
        {
            // 1. Hämta alla rutor i det aktuella huset
            List<SudokuTile> houseTiles = GetHouseTiles(grid, houseIndex, houseType);

            // 2. Samla alla tillgängliga kandidatsiffror (1-9) som fortfarande finns kvar i husets tomma rutor
            List<int> availableDigits = new List<int>();
            for (int d = 1; d <= 9; d++)
            {
                if (houseTiles.Any(t => !t.Used && t.Candidates.Contains(d)))
                {
                    availableDigits.Add(d);
                }
            }

            // Om vi har färre unika siffror kvar än storleken på vår dolda grupp, hoppa över huset
            if (availableDigits.Count < multCount) continue;

            // 3. Generera alla möjliga kombinationer av siffror med storleken 'multCount' (t.ex. par eller tripplar av siffror)
            List<List<int>> digitCombinations = new List<List<int>>();
            GenerateDigitCombinations(availableDigits, multCount, 0, new List<int>(), digitCombinations);

            // 4. Utvärdera varje sifferkombination
            foreach (var digitGroup in digitCombinations)
            {
                // Hitta alla celler i huset som innehåller MINST EN av siffrorna i vår siffergrupp
                List<SudokuTile> tilesContainingDigits = houseTiles
                    .Where(t => !t.Used && t.Candidates.Overlaps(digitGroup))
                    .ToList();

                // GULDREGELN FÖR HIDDEN MULTIPLES:
                // Siffergruppen är en Hidden Multiple OM och endast OM dessa siffror är 
                // totalt isolerade till exakt lika många celler som antalet siffror i gruppen!
                if (tilesContainingDigits.Count != multCount) continue;

                // 5. Kontrollera om det finns ANDRA kandidater i dessa celler som kan rensas bort
                List<TileIndex> triggerIndexes = tilesContainingDigits.Select(t => t.index).ToList();
                List<int> candidatesToRemove = new List<int>();

                foreach (var tile in tilesContainingDigits)
                {
                    // I en Hidden Multiple är det de kandidater som INTE tillhör digitGroup som ska rensas bort
                    var illegalCandidates = tile.Candidates.Where(c => !digitGroup.Contains(c)).ToList();
                    foreach (var c in illegalCandidates)
                    {
                        candidatesToRemove.Add(c);
                    }
                }

                // Om vi hittade kandidater som faktiskt kan tas bort, har vi en giltig Hidden Multiple!
                if (candidatesToRemove.Count > 0)
                {
                    List<TileIndex> fullHouse = houseTiles.Select(t => t.index).ToList();
                    HashSet<int> candidateSet = new HashSet<int>(candidatesToRemove);

                    // Skicka all strukturerad information till ditt Hint-GUI via din konstruktor
                    solveInformation = new CandidateSolveInformation(
                        triggerIndexes,       // De celler där det dolda paret/trippeln bor (Triggers)
                        candidateSet,         // De kandidater som rensas bort (Removals)
                        triggerIndexes,       // I Hidden Multiples är trigger-cellerna och raderings-cellerna samma fysiska celler!
                        fullHouse,            // Det påverkade husets index för bakgrundsljus
                        houseType             // Hustypen
                    );
                    return true;
                }
            }
        }

        return false;
    }

    #region Geometri och Kombination-Hjälpare

    private List<SudokuTile> GetHouseTiles(SudokuGrid9x9 grid, int houseIndex, HouseType type)
    {
        List<SudokuTile> tiles = new List<SudokuTile>();
        switch (type)
        {
            case HouseType.Row:
                for (int i = 0; i < 9; i++) tiles.Add(grid[houseIndex, i]);
                break;
            
            case HouseType.Column:
                for (int i = 0; i < 9; i++) tiles.Add(grid[i, houseIndex]);
                break;
            
            case HouseType.Box:
                int startRow = (houseIndex / 3) * 3;
                int startCol = (houseIndex % 3) * 3;
            
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        tiles.Add(grid[startRow + r, startCol + c]);
                    }
                }
                break;
        }
        return tiles;
    }


    // En ren, rekursiv metod för att hitta kombinationer av siffror (ersätter FindAllCombinations)
    private void GenerateDigitCombinations(List<int> source, int k, int start, List<int> current, List<List<int>> result)
    {
        if (current.Count == k)
        {
            result.Add(new List<int>(current));
            return;
        }

        for (int i = start; i < source.Count; i++)
        {
            current.Add(source[i]);
            GenerateDigitCombinations(source, k, i + 1, current, result);
            current.RemoveAt(current.Count - 1); // Backtrack
        }
    }
    #endregion
}
