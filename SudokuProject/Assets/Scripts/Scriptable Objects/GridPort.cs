using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/GridPort")]
public class GridPort : ScriptableObject
{
    public SudokuGrid9x9 grid { get; private set; }
    public bool gridContradicted { get; private set; }

    public TileBehaviour[,] tileBehaviours { get; private set; }= new TileBehaviour[9,9];

    public UnityAction<bool> OnContradictionStatusUpdate;
    public UnityAction OnRequestGrid;

    public void RequestGrid()
    {
        OnRequestGrid?.Invoke();
    }

    public void SendGridCopy(SudokuGrid9x9 gridCopy, TileBehaviour[,] tileBehaviours)
    {
        grid = gridCopy;
        this.tileBehaviours = tileBehaviours;
    }
    
    public void UpdateContradictionStatus(bool gridHasContradiction)
    {
        OnContradictionStatusUpdate?.Invoke(gridHasContradiction);
        gridContradicted = gridHasContradiction;
    }

    public string GetGridAsSeed()
    {
        return grid.AsString();
    }

    public void Reset()
    {
        gridContradicted = false;
    }
}