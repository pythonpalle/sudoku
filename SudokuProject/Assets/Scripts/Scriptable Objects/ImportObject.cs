using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/ImportObject")]
public class ImportObject : ScriptableObject
{
    public UnityAction<SudokuGrid9x9> OnGridImported;
    public bool isSelected;

    public void ImportGrid(SudokuGrid9x9 grid)
    {
        OnGridImported?.Invoke(grid);
    }
}
