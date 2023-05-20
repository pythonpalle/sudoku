using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.WSA;

public struct Fish
{
    public List<MultiCombo> combos;

    public Fish(List<MultiCombo> combos)
    {
        this.combos = combos;
    }

    public int Size => combos.Count;
}

public class FishMethod : CandidateMethod
{
    protected bool TryFindFish(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return TryFindFishInRow(grid, multCount, out removal)
               || TryFindFishInCol(grid, multCount, out removal);
    }
    
    protected bool TryFindFishInRow(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromFishInRowCol(grid, multCount, true, out removal);
    }
    
    protected bool TryFindFishInCol(SudokuGrid9x9 grid, int multCount, out CandidateRemoval removal)
    {
        return CandidatesFromFishInRowCol(grid, multCount, false, out removal);
    }
    
    private bool CandidatesFromFishInRowCol(SudokuGrid9x9 grid, int multCount, bool fishInRow, out CandidateRemoval removal)
    {
        removal = new CandidateRemoval();

        for (int digit = 1; digit <= 9; digit++)
        {
            // (int: rad (col), List: columner i raden som digit finns i)
            Dictionary<int, List<int>> rowCandidates = new Dictionary<int, List<int>>();
            for (int row = 0; row < 9; row++)
            {
                rowCandidates.Add(row, new List<int>());
                
                for (int col = 0; col < 9; col++)
                {
                    var tile = fishInRow ? grid[row, col] : grid[col, row];
                    if (!tile.Used && tile.Candidates.Contains(digit))
                    {
                        rowCandidates[row].Add(col);
                    }
                }
                
                // only point of including row if number of occurences for digit is between 2 and _multCount_
                if (rowCandidates[row].Count > multCount || rowCandidates[row].Count < 2)
                {
                    rowCandidates.Remove(row);
                }
            }
            
            // can't be fish in row if less then _multCount_ rows
            if (rowCandidates.Count < multCount)
            {
                continue;
            }

            List<List<TileIndex>> combinations = new List<List<TileIndex>>();
            List<KeyValuePair<int, List<int>>> rowLists = rowCandidates.ToList();

            int n = rowLists.Count;
            int k = multCount;
            
            KeyValuePair<int, List<int>>[] tempList = new KeyValuePair<int, List<int>>[k];

            FindAllCombinations(combinations, rowLists, tempList, grid, rowCandidates, digit, fishInRow, 0, n-1, 0, k);
            foreach (var effectedTileList in combinations)
            {
                if (effectedTileList.Count > 0)
                {
                    removal.indexes = effectedTileList;
                    removal.candidateSet = new HashSet<int>{digit};
                    return true;
                }
            }
        }

        return false;
    }
    
    private void FindAllCombinations(List<List<TileIndex>> combinations, List<KeyValuePair<int, List<int>>> colNumbers,
        KeyValuePair<int, List<int>>[] tempList, SudokuGrid9x9 grid, Dictionary<int, List<int>> rowCandidates, int digit, bool fishRow, int start, int end,
        int index, int k)
    {
        // from https://www.geeksforgeeks.org/print-all-possible-combinations-of-r-elements-in-a-given-array-of-size-n/
        
        if (index == k)
        {
            if (ValidCombination(tempList, k))
            {
                List<TileIndex> indices = GetIndicesFromTiles(grid, tempList, fishRow, digit);
                
                // only bother including it if it has indices
                if (indices.Count > 0)
                {
                    combinations.Add(indices);
                }
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = colNumbers[i];
            FindAllCombinations(combinations, colNumbers, tempList, grid, rowCandidates, digit, fishRow, i + 1, end, index + 1, k);
        }
    }

    bool ValidCombination(KeyValuePair<int, List<int>>[] tempList, int k)
    {
        HashSet<int> allColumns = new HashSet<int>();
        HashSet<int> allRows = new HashSet<int>();

        foreach (var pair in tempList)
        {
            Assert.IsFalse(allRows.Contains(pair.Key));
            
            allRows.Add(pair.Key);
            
            foreach (var col in pair.Value)
            {
                allColumns.Add(col);
            }
        }

        bool rightAmountOfColumns = allColumns.Count == k;
        // if (rightAmountOfColumns)
        // {
        //     Debug.Log("Valid fish: ");
        //     foreach (var pair in tempList)
        //     {
        //         string rowString = "";
        //         foreach (var col in pair.Value)
        //         {
        //             rowString += $"{col}, ";
        //         }
        //         
        //         Debug.Log($"Row: {pair.Key}. Cols: {rowString}");
        //     }
        // }
        
        return rightAmountOfColumns;
    }
    
    List<TileIndex> GetIndicesFromTiles(SudokuGrid9x9 grid, KeyValuePair<int, List<int>>[] tempList, bool fishRow, int digit)
    {
        List<TileIndex> effectedIndices = new List<TileIndex>();

        HashSet<int> colSet = new HashSet<int>();
        HashSet<int> invalidRows = new HashSet<int>();

        foreach (var pair in tempList)
        {
            invalidRows.Add(pair.Key);
            colSet.UnionWith(pair.Value);
        }

        Assert.AreEqual(colSet.Count, tempList.Length);

        
        foreach (int col in colSet)
        {
            
            for (int row = 0; row < 9; row++)
            {
                // skip tiles in fish
                if (invalidRows.Contains(row))
                    continue;

                var potentialTile = fishRow ? grid[row, col] : grid[col, row];
                if (!potentialTile.Used && potentialTile.Candidates.Contains(digit))
                {
                    effectedIndices.Add(potentialTile.index);
                }
                
            }
        }
        
        return effectedIndices;
    }

    // private bool CandidatesFromFishInRowCol(SudokuGrid9x9 grid, int multCount, bool fishInRow, out CandidateRemoval removal)
    // {
    //     List<MultiCombo> allCombos = new List<MultiCombo>();
    //
    //     removal = new CandidateRemoval();
    //
    //     var nonUseTiles = new List<SudokuTile>();
    //     
    //     for (int row = 0; row < 9; row++)
    //     {
    //         nonUseTiles.Clear();
    //
    //         for (int col = 0; col < 9; col++)
    //         {
    //             // invert index if checking column
    //             var tile = fishInRow ? grid[row, col] : grid[col, row];
    //
    //             if (tile.Used) 
    //                 continue;
    //             
    //             nonUseTiles.Add(tile);
    //         }
    //         
    //         if (nonUseTiles.Count < multCount) continue;
    //         
    //         // key: candidate,
    //         // value: tiles with that candidate
    //         Dictionary<int, List<TileIndex>> candidateCount = new Dictionary<int, List<TileIndex>>();
    //
    //         // store witch tiles contain each index
    //         for (int candidate = 1; candidate <= 9; candidate++)
    //         {
    //             for (int i = 0; i < nonUseTiles.Count; i++)
    //             {
    //                 if (nonUseTiles[i].Candidates.Contains(candidate))
    //                 {
    //                     if (candidateCount.ContainsKey(candidate))
    //                     {
    //                         candidateCount[candidate].Add(nonUseTiles[i].index);
    //                     }
    //                     else
    //                     {
    //                         candidateCount.Add(candidate, new List<TileIndex>{nonUseTiles[i].index});
    //
    //                         // remove entry if more then multCount tiles share candidate
    //                         if (candidateCount[candidate].Count > multCount)
    //                         {
    //                             candidateCount.Remove(candidate);
    //                             break;
    //                         }
    //                     }
    //                 }
    //             }
    //             
    //             List<MultiCombo> rowCombos = new List<MultiCombo>();
    //             int[] tempList = new int[multCount];
    //             List<int> numbers = candidateCount.Keys.ToList();
    //
    //             FindAllCombinations(rowCombos, numbers, candidateCount, tempList, 0, numbers.Count - 1, 0, multCount);
    //             if (rowCombos.Count > 0)
    //                 allCombos.AddRange(rowCombos);
    //         }
    //
    //         
    //     }
    //     
    //     List<Fish> fishes = new List<Fish>();
    //
    //     int k = multCount;
    //     int n = allCombos.Count;
    //
    //     if (n < k)
    //     {
    //         return false;
    //     }
    //     
    //     MultiCombo[] tempComboList = new MultiCombo[k];
    //
    //     FindAllFishes(fishes, allCombos, tempComboList, 0, n - 1, 0, k, fishInRow);
    //
    //     foreach (var fish in fishes)
    //     {
    //         if (EffectsRowColTiles(grid, fish, fishInRow, out List<TileIndex> effectedIndices))
    //         {
    //             removal.indexes = effectedIndices;
    //             removal.candidateSet = fish.combos[0].candidates;
    //             return true;
    //         }
    //     }
    //
    //
    //     return false;
    // }
    //
    // private void FindAllFishes(List<Fish> fishes, List<MultiCombo> allCombos, MultiCombo[] tempComboList, int start, int end, int index, int k, bool checkRow)
    // {
    //     if (index >= k)
    //     {
    //         if (CombosMakesValidFish(tempComboList, checkRow, out Fish fish))
    //         {
    //             fishes.Add(fish);
    //         }
    //         
    //         return;
    //     }
    //     
    //     for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
    //     {
    //         tempComboList[index] = allCombos[i];
    //         FindAllFishes(fishes, allCombos, tempComboList, i + 1, end, index + 1, k, checkRow);
    //     }
    // }
    //
    // private bool CombosMakesValidFish(MultiCombo[] tempComboList, bool checkRow, out Fish fish)
    // {
    //     fish = new Fish();
    //     
    //     HashSet<int> candidateSet = tempComboList[0].candidates;
    //
    //     for (int i = 1; i < tempComboList.Length; i++)
    //     {
    //         var comboList = tempComboList[i];
    //         var cands = comboList.candidates;
    //
    //         if (cands == null)
    //         {
    //             Debug.Log("HERE");
    //         }
    //         
    //         int maxCand = cands.Max();
    //         
    //         // not valid fish if they dont share same indices
    //         if (tempComboList[i].candidates.SetEquals(candidateSet)) // bug: null reference
    //             return false;
    //     }
    //
    //     // check number of indices per row / col
    //     Dictionary<int, int> candCount = new Dictionary<int, int>();
    //     foreach (var combo in tempComboList)
    //     {
    //         foreach (var index in combo.tileIndices)
    //         {
    //             int candidate = checkRow ? index.col : index.row;
    //
    //             if (!candCount.ContainsKey(candidate))
    //                 candCount.Add(candidate, 1);
    //             else
    //                 candCount[candidate]++;
    //         }
    //     }
    //
    //     foreach (var count in candCount.Values)
    //     {
    //         if (count != tempComboList.Length)
    //             return false;
    //     }
    //     
    //     // if neither of either checks fail, the fish should be valid
    //     Debug.Log("Found valid fish of size " + tempComboList.Length + "!");
    //     fish.combos = new List<MultiCombo>(tempComboList);
    //     return true;
    // }
    //
    // private bool EffectsRowColTiles(SudokuGrid9x9 grid, Fish fish, bool fishInRow, out List<TileIndex> effectIndices)
    // {
    //     // if fishInRow - effects columns
    //
    //     effectIndices = new List<TileIndex>();
    //
    //     List<int> rowNumbers = new List<int>();
    //     List<int> ignoreNumbers = new List<int>();
    //
    //     HashSet<int> candidates = fish.combos[0].candidates;
    //
    //
    //     // get col numbers
    //     foreach (var index in fish.combos[0].tileIndices)
    //     {
    //         if (fishInRow)
    //         {
    //             rowNumbers.Add(index.col);
    //         }
    //         else
    //         {
    //             rowNumbers.Add(index.row);
    //         }
    //     }
    //     
    //     // get ignore numbers
    //     
    //     // if fish in row, ignore numbers are the rows
    //     if (fishInRow)
    //     {
    //         foreach (var combo in fish.combos)
    //         {
    //             ignoreNumbers.Add(combo.tileIndices[0].row);
    //         }
    //     }
    //     
    //     // if fish in col, ignore numbers are the cols
    //     else
    //     {
    //         foreach (var index in fish.combos[0].tileIndices)
    //         {
    //             rowNumbers.Add(index.col);
    //             rowNumbers.Add(index.row);
    //         }
    //     }
    //     
    //     
    //     bool effectsSomeTiles = false;
    //
    //     if (fishInRow)
    //     {
    //         for (int col = 0; col < 9; col++)
    //         {
    //             if (ignoreNumbers.Contains(col)) continue;
    //
    //             foreach (var row in rowNumbers)
    //             {
    //                 var tile = grid[row, col];
    //                 foreach (var candidate in tile.Candidates)
    //                 {
    //                     if (candidates.Contains(candidate))
    //                     {
    //                         effectsSomeTiles = true;
    //                         effectIndices.Add(tile.index);
    //                     }
    //                 }
    //             }
    //         } 
    //     }
    //
    //     return effectsSomeTiles;
    // }
}
