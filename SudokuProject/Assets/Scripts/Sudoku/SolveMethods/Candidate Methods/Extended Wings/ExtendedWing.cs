using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ExtendedWing : CandidateMethod
{
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Extreme;

    // Gemensam, optimerad motor för både XY-Wing och XYZ-Wing
    protected bool SearchWingCandidates(SudokuGrid9x9 grid, bool isXyzWing, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // 1. Förbered listor över bivalue (2 kandidater) och trivalue (3 kandidater) celler
        List<SudokuTile> bivalueTiles = new List<SudokuTile>();
        List<SudokuTile> trivalueTiles = new List<SudokuTile>();

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var tile = grid[r, c];
                if (tile.Used) continue;
                if (tile.Candidates.Count == 2) bivalueTiles.Add(tile);
                else if (tile.Candidates.Count == 3) trivalueTiles.Add(tile);
            }
        }

        // Välj rätt typ av Pivot-celler baserat på vingen
        List<SudokuTile> pivotTiles = isXyzWing ? trivalueTiles : bivalueTiles;

        // 2. Loopa igenom alla potentiella Pivot-celler (Base)
        foreach (var pivot in pivotTiles)
        {
            var pivotCand = pivot.Candidates;

            // Hämta alla bivalue-celler som SER vår pivot (potentiella vingar)
            List<SudokuTile> visibleWings = bivalueTiles
                .Where(w => w.index != pivot.index && TilesIntersect(pivot.index, w.index))
                .ToList();

            if (visibleWings.Count < 2) continue;

            // 3. Loopa igenom kombinationer av två vingar (Wing 1 och Wing 2)
            for (int i = 0; i < visibleWings.Count; i++)
            {
                var wing1 = visibleWings[i];
                var w1Cand = wing1.Candidates;

                // Räkna ut gemensamma kandidater mellan Pivot och Wing 1
                var pivotW1Intersect = pivotCand.Intersect(w1Cand).ToList();
                
                // Krav: De måste dela exakt 2 kandidater (XYZ-Wing) eller 1 kandidat (XY-Wing)
                int requiredShared = isXyzWing ? 2 : 1;
                if (pivotW1Intersect.Count != requiredShared) continue;

                for (int j = i + 1; j < visibleWings.Count; j++)
                {
                    var wing2 = visibleWings[j];
                    var w2Cand = wing2.Candidates;

                    // Wing 1 och Wing 2 får inte vara samma eller ha exakt samma kandidater
                    if (w1Cand.SetEquals(w2Cand)) continue;

                    // Räkna ut gemensamma kandidater mellan Pivot och Wing 2
                    var pivotW2Intersect = pivotCand.Intersect(w2Cand).ToList();
                    if (pivotW2Intersect.Count != requiredShared) continue;

                    // Hitta den delade målkandidaten (Z) som finns i BÅDA vingarna, men INTE delas mellan dem i pivoten
                    var wingIntersect = w1Cand.Intersect(w2Cand).ToList();
                    if (wingIntersect.Count != 1) continue; // Vingarna måste dela exakt EN kandidat (Z)

                    int zCandidate = wingIntersect[0];

                    // För XY-Wing får Z inte vara en del av de kandidater som Pivot delar med vingarna som bas
                    if (!isXyzWing && pivotCand.Contains(zCandidate)) continue;
                    // För XYZ-Wing MÅSTE Z finnas i Pivoten också
                    if (isXyzWing && !pivotCand.Contains(zCandidate)) continue;

                    // 4. Hitta celler som påverkas (ser vingarna och innehåller Z)
                    List<TileIndex> removalIndexes = new List<TileIndex>();

                    // Hämta alla celler på brädet
                    for (int r = 0; r < 9; r++)
                    {
                        for (int c = 0; c < 9; c++)
                        {
                            var target = grid[r, c];
                            if (target.Used || !target.Candidates.Contains(zCandidate)) continue;
                            if (target.index == pivot.index || target.index == wing1.index || target.index == wing2.index) continue;

                            // Grundkrav: Cellen måste se BÅDA vingarna
                            bool seesWing1 = TilesIntersect(target.index, wing1.index);
                            bool seesWing2 = TilesIntersect(target.index, wing2.index);

                            if (seesWing1 && seesWing2)
                            {
                                // För XYZ-Wing måste målcellen dessutom se Pivot-cellen också!
                                if (isXyzWing && !TilesIntersect(target.index, pivot.index)) continue;

                                removalIndexes.Add(target.index);
                            }
                        }
                    }

                    // 5. VINGE HITTAD! Paketera all data till ditt UI
                    if (removalIndexes.Count > 0)
                    {
                        // Trigger-celler är hela ving-familjen: Pivot + Wing 1 + Wing 2
                        List<TileIndex> triggerIndexes = new List<TileIndex> { pivot.index, wing1.index, wing2.index };
                        
                        // FullHouseVisualIndexes används här för att lysa upp hela sammanhanget (ving-klustret)
                        List<TileIndex> fullVisualCluster = new List<TileIndex> { pivot.index, wing1.index, wing2.index };
                        fullVisualCluster.AddRange(removalIndexes);
                        
                        Debug.Log("Extended wing found");

                        solveInformation = new CandidateSolveInformation(
                            removalIndexes,
                            new HashSet<int> { zCandidate },
                            triggerIndexes,
                            fullVisualCluster,
                            HouseType.Box // Vi sätter Box som standard då vingar oftast spänner över box-gränser
                        );

                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Geometrisk hjälpfunktion för att se om två celler delar rad, kolumn eller box
    private bool TilesIntersect(TileIndex a, TileIndex b)
    {
        if (a.row == b.row || a.col == b.col) return true;
        return (a.row / 3 == b.row / 3) && (a.col / 3 == b.col / 3);
    }
}
