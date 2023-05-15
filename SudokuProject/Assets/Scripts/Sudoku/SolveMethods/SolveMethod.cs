using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SolveMethod : WFCGridSolver
{
    public virtual bool TryMakeProgress(SudokuGrid9x9 inGrid)
    {
        return false;
    }
}
