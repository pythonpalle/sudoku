using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SolveMethod : WFCGridSolver
{
    public virtual bool TryMakeProgress(SudokuGrid9x9 inGrid, out SudokuGrid9x9 outGrid)
    {
        outGrid = new SudokuGrid9x9();
        return false;
    }
}
