using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class LockedCandidatesMethod : CandidateMethod
{
    // EN ENHETLIG MOTOR FÖR BÅDE POINTING OCH CLAIMING
    protected bool SearchLockedCandidates(SudokuGrid9x9 grid, int pointers, bool findPointing, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        for (int candidate = 1; candidate <= 9; candidate++)
        {
            // Vi kör igenom alla 9 hus av vald typ (0-8 rader/kolumner eller 0-8 boxar)
            for (int i = 0; i < 9; i++)
            {
                // Hämta rutorna i det hus vi undersöker (Källan)
                List<SudokuTile> sourceTiles = findPointing ? GetBoxTiles(grid, i) : GetLineTiles(grid, i, isRow: true); 
                // Om findPointing är falskt körs rader först, vi kan lägga till kolumner i loopen efter
                
                if (ProcessHouse(grid, sourceTiles, candidate, pointers, findPointing, out solveInformation)) 
                    return true;

                if (!findPointing)
                {
                    List<SudokuTile> colTiles = GetLineTiles(grid, i, isRow: false);
                    if (ProcessHouse(grid, colTiles, candidate, pointers, findPointing, out solveInformation)) 
                        return true;
                }
            }
        }
        return false;
    }

    private bool ProcessHouse(SudokuGrid9x9 grid, List<SudokuTile> sourceTiles, int candidate, int pointers, bool findPointing, out CandidateSolveInformation solveInformation)
    {
        solveInformation = new CandidateSolveInformation();

        // Hitta alla rutor i huset som faktiskt har denna kandidat tillgänglig
        List<TileIndex> candidateIndices = sourceTiles
            .Where(t => !t.Used && t.Candidates.Contains(candidate))
            .Select(t => t.index)
            .ToList();

        // Sudoku-regel: Om antalet matchningar inte stämmer med antalet sökta pointers (2 eller 3), gå vidare
        if (candidateIndices.Count != pointers) return false;

        // Kolla om alla dessa kandidat-rutor delar ett sekundärt hus
        bool sameRow = candidateIndices.All(idx => idx.row == candidateIndices[0].row);
        bool sameCol = candidateIndices.All(idx => idx.col == candidateIndices[0].col);
        bool sameBox = candidateIndices.All(idx => GetBoxIndex(idx) == GetBoxIndex(candidateIndices[0]));

        List<SudokuTile> targetHouse = null;

        if (findPointing) // Box to Row/Col (Pointing)
        {
            if (sameRow) targetHouse = GetLineTiles(grid, candidateIndices[0].row, isRow: true);
            else if (sameCol) targetHouse = GetLineTiles(grid, candidateIndices[0].col, isRow: false);
        }
        else // Row/Col to Box (Claiming)
        {
            if (sameBox) targetHouse = GetBoxTiles(grid, GetBoxIndex(candidateIndices[0]));
        }

        if (targetHouse == null) return false;

        // Hitta rutor i målhuset som kan rensas (de får inte vara källrutorna själva)
        List<TileIndex> eliminationIndices = targetHouse
            .Where(t => !t.Used && !candidateIndices.Contains(t.index) && t.Candidates.Contains(candidate))
            .Select(t => t.index)
            .ToList();

        // Om vi hittade rutor att rensa har vi en giltig exkludering!
        if (eliminationIndices.Count > 0)
        {
            var fullHouse = targetHouse.Select(t => t.index).ToList();
            var houseType = findPointing 
                ? (sameRow ? HouseType.Row : HouseType.Column) 
                : HouseType.Box;
            
            solveInformation = new CandidateSolveInformation(eliminationIndices, candidate, candidateIndices, fullHouse, houseType);
            return true;
        }

        return false;
    }

    #region Effektiva Geometri-Hjälpare (Slipper nästlade loopar i algoritmen)
    
    private List<SudokuTile> GetLineTiles(SudokuGrid9x9 grid, int lineIndex, bool isRow)
    {
        List<SudokuTile> tiles = new List<SudokuTile>();
        for (int i = 0; i < 9; i++)
        {
            tiles.Add(isRow ? grid[lineIndex, i] : grid[i, lineIndex]);
        }
        return tiles;
    }

    private List<SudokuTile> GetBoxTiles(SudokuGrid9x9 grid, int boxIndex)
    {
        List<SudokuTile> tiles = new List<SudokuTile>();
        int startRow = (boxIndex / 3) * 3;
        int startCol = (boxIndex % 3) * 3;

        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                tiles.Add(grid[startRow + r, startCol + c]);

        return tiles;
    }

    private int GetBoxIndex(TileIndex index)
    {
        return (index.row / 3) * 3 + (index.col / 3);
    }
    #endregion
}
