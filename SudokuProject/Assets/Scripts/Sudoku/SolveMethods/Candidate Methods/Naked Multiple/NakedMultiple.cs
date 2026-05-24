using System.Collections.Generic;
using System.Linq;

public abstract class NakedMultiple : CandidateMethod
{
    protected abstract int multCount { get; }
    
    // Den centrala sök-motorn för BÅDE Rader, Kolumner och Boxar
    
    protected bool SearchNakedMultiples(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        // 1. Sök i rader
        if (SearchNakedMultiples(grid, HouseType.Row, out solveInformation)) return true;
        
        // 2. Sök i kolumner
        if (SearchNakedMultiples(grid, HouseType.Column, out solveInformation)) return true;
        
        // 3. Sök i boxar
        if (SearchNakedMultiples(grid, HouseType.Box, out solveInformation)) return true;

        return false;    
    }
    
    private bool SearchNakedMultiples(SudokuGrid9x9 grid, HouseType houseType, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // Loopa igenom de 9 husen (0-8) av den valda typen
        for (int houseIndex = 0; houseIndex < 9; houseIndex++)
        {
            // 1. Hämta alla rutor i det aktuella huset
            List<SudokuTile> houseTiles = GetHouseTiles(grid, houseIndex, houseType);

            // 2. Filtrera ut rutor som är tomma och har max 'multCount' antal kandidater (Entropi)
            List<SudokuTile> validTiles = houseTiles
                .Where(t => !t.Used && t.Candidates.Count > 1 && t.Candidates.Count <= multCount)
                .ToList();

            // Om vi har färre rutor än storleken på vår grupp (t.ex. mindre än 3 rutor för en Triple), hoppa över
            if (validTiles.Count < multCount) continue;

            // 3. Generera alla möjliga kombinationer av dessa rutor med storleken 'multCount'
            List<List<SudokuTile>> combinations = new List<List<SudokuTile>>();
            GenerateCombinations(validTiles, multCount, 0, new List<SudokuTile>(), combinations);

            // 4. Utvärdera varje kombination
            foreach (var combination in combinations)
            {
                // Samla alla unika kandidater som denna grupp av rutor innehåller
                HashSet<int> sharedCandidates = new HashSet<int>();
                foreach (var tile in combination)
                {
                    sharedCandidates.UnionWith(tile.Candidates);
                }

                // GULDREGELN: En Naked Multiple existerar BARA om antalet unika kandidater 
                // är EXAKT lika med antalet rutor i kombinationen!
                if (sharedCandidates.Count != multCount) continue;

                // 5. Hitta rutor i resten av HUSET som har dessa kandidater och kan rensas
                List<TileIndex> triggerIndexes = combination.Select(t => t.index).ToList();
                List<TileIndex> removalIndexes = new List<TileIndex>();
                List<int> candidatesToRemove = new List<int>();

                foreach (var houseTile in houseTiles)
                {
                    // Vi kan inte rensa i de rutor som utgör själva ledtråden (våra triggers)
                    if (triggerIndexes.Contains(houseTile.index) || houseTile.Used) continue;

                    // Hitta vilka av de delade kandidaterna som finns i denna ruta
                    var overlaps = houseTile.Candidates.Intersect(sharedCandidates).ToList();
                    if (overlaps.Count > 0)
                    {
                        removalIndexes.Add(houseTile.index);
                        foreach (var c in overlaps) candidatesToRemove.Add(c);
                    }
                }

                // Om vi faktiskt hittade kandidater att rensa har vi en giltig lösning!
                if (removalIndexes.Count > 0)
                {
                    List<TileIndex> fullHouse = houseTiles.Select(t => t.index).ToList();
                    HashSet<int> candidateSet = new HashSet<int>(candidatesToRemove);

                    // Returnera all data inklusive den visuella kontexten till ditt GUI!
                    solveInformation = new CandidateSolveInformation(
                        removalIndexes, 
                        candidateSet, 
                        triggerIndexes, 
                        fullHouse, 
                        houseType
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
                    for (int c = 0; c < 3; c++)
                        tiles.Add(grid[startRow + r, startCol + c]);
                break;
        }
        return tiles;
    }

    // En ren och effektiv rekursiv metod för att hitta kombinationer (ersätter FindAllCombinations)
    private void GenerateCombinations(List<SudokuTile> source, int k, int start, List<SudokuTile> current, List<List<SudokuTile>> result)
    {
        if (current.Count == k)
        {
            result.Add(new List<SudokuTile>(current));
            return;
        }

        for (int i = start; i < source.Count; i++)
        {
            current.Add(source[i]);
            GenerateCombinations(source, k, i + 1, current, result);
            current.RemoveAt(current.Count - 1); // Backtrack
        }
    }
    #endregion
}
