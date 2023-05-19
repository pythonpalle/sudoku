using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMethod : CandidateMethod
{
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
        /*
         
         diciionary candCount = int (siffra) : int (antal av siffran i kolumnen)
        för varje rad:
            för varje siffra:
                för varje kolumn:
                    tile = rad, col
                    om tile har kandidat siffra:
                        candCount[siffra]++;
        
        
        */
        throw new System.NotImplementedException();
    }
}
