using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniquenessRectangle : CandidateMethod
{
    public override string GetName => "Unique Rectangle (Type 1)";
    public override PuzzleDifficulty Difficulty => PuzzleDifficulty.Extreme;

    public override bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // 1. Hämta alla tomma rutor som har EXAKT 2 kandidater
        List<SudokuTile> bivalueTiles = new List<SudokuTile>();
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                var tile = grid[r, c];
                if (!tile.Used && tile.Candidates.Count == 2)
                {
                    bivalueTiles.Add(tile);
                }
            }
        }

        // 2. Loopa igenom varje par av bivalue-rutor för att försöka hitta basen (Hörn 1 och Hörn 2)
        for (int i = 0; i < bivalueTiles.Count; i++)
        {
            var tile1 = bivalueTiles[i];
            var candSet = tile1.Candidates; // De två dödliga kandidaterna (t.ex. {2, 5})

            for (int j = i + 1; j < bivalueTiles.Count; j++)
            {
                var tile2 = bivalueTiles[j];

                // De två första hörnen måste ha exakt samma två kandidater
                if (!tile2.Candidates.SetEquals(candSet)) continue;

                // De måste dela antingen samma rad eller samma kolumn
                bool shareRow = tile1.index.row == tile2.index.row;
                bool shareCol = tile1.index.col == tile2.index.col;
                if (!shareRow && !shareCol) continue;

                // Sudoku-regel för Unique Rectangle: Minst ett par av parallella hörn MÅSTE dela box
                if (!InSameBox(tile1.index, tile2.index)) continue;

                foreach (var tile3 in bivalueTiles)
                {
                    if (tile3.index == tile1.index || tile3.index == tile2.index) continue;
                    if (!tile3.Candidates.SetEquals(candSet)) continue;

                    int targetRow, targetCol;

                    if (shareRow) // Hörn 1 och Hörn 2 delar RAD
                    {
                        // Hörn 3 måste ligga på samma KOLUMN som antingen Hörn 1 eller Hörn 2
                        if (tile3.index.col == tile1.index.col)
                        {
                            targetRow = tile3.index.row;
                            targetCol = tile2.index.col; // Det fjärde hörnet hamnar under Hörn 2
                        }
                        else if (tile3.index.col == tile2.index.col)
                        {
                            targetRow = tile3.index.row;
                            targetCol = tile1.index.col; // Det fjärde hörnet hamnar under Hörn 1
                        }
                        else continue;
                    }
                    else // shareCol - Hörn 1 och Hörn 2 delar KOLUMN
                    {
                        // Hörn 3 måste ligga på samma RAD som antingen Hörn 1 eller Hörn 2
                        if (tile3.index.row == tile1.index.row)
                        {
                            targetRow = tile2.index.row; // Det fjärde hörnet hamnar bredvid Hörn 2
                            targetCol = tile3.index.col;
                        }
                        else if (tile3.index.row == tile2.index.row)
                        {
                            targetRow = tile1.index.row; // Det fjärde hörnet hamnar bredvid Hörn 1
                            targetCol = tile3.index.col;
                        }
                        else continue;
                    }

                    // 4. Validera det fjärde hörnet (vår unika drabbade ruta)
                    var targetTile = grid[targetRow, targetCol];

                    if (targetTile.Used) continue;
                    if (!candSet.IsSubsetOf(targetTile.Candidates)) continue;

                    // Sudoku-regel: De två hörnpar som ligger parallellt vertikalt eller horisontellt måste dela box
                    // Vi kontrollerar att vår nya målruta delar box med sitt parallella hörn (tile3)
                    if (!InSameBox(tile3.index, targetTile.index)) continue;

                    // 5. UNIQUE RECTANGLE HITTAD!
                    List<TileIndex> triggerIndexes = new List<TileIndex> { tile1.index, tile2.index, tile3.index };
                    List<TileIndex> removalIndexes = new List<TileIndex> { targetTile.index };
                    List<TileIndex> rectangleFrame = new List<TileIndex> { tile1.index, tile2.index, tile3.index, targetTile.index };

                    solveInformation = new CandidateSolveInformation(
                        removalIndexes, 
                        new HashSet<int>(candSet), 
                        triggerIndexes, 
                        rectangleFrame, 
                        HouseType.Box
                    );

                    return true;
                }
            }
        }

        return false;
    }

    private bool InSameBox(TileIndex index1, TileIndex index2)
    {
        return (index1.row / 3 == index2.row / 3) && (index1.col / 3 == index2.col / 3);
    }

    private void DebugRectangle(TileIndex t1, TileIndex t2, TileIndex t3, TileIndex target, HashSet<int> candidates)
    {
        string candStr = string.Join(", ", candidates);
        Debug.LogWarning($"[Unique Rectangle] Hittad med kandidaterna {{{candStr}}}!\n" +
                         $"Hörn 1: {t1}, Hörn 2: {t2}, Hörn 3: {t3} -> Rensar i Hörn 4 (Mål): {target}");
    }
}
