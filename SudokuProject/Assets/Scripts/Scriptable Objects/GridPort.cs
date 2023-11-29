using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/GridPort")]
public class GridPort : ScriptableObject
{
    public SudokuGrid9x9 grid { get; private set; }
    public bool gridContradicted { get; private set; }
    
    public UnityAction<bool> OnContradictionStatusUpdate;
    public UnityAction OnRequestGrid;

    public void RequestGrid()
    {
        OnRequestGrid?.Invoke();
    }

    public void SendGridCopy(SudokuGrid9x9 gridCopy)
    {
        grid = gridCopy;
    }
    
    public void UpdateContradictionStatus(bool gridHasContradiction)
    {
        OnContradictionStatusUpdate?.Invoke(gridHasContradiction);
        gridContradicted = gridHasContradiction;
    }
}