using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/HintObject")]
public class HintObject : ScriptableObject
{
    public UnityAction OnRequestGrid;
    public UnityAction<SudokuGrid9x9, bool> OnSendGridCopy;
    public UnityAction<TileIndex> OnHintFound;

    public void RequestGrid()
    {
        OnRequestGrid?.Invoke();
    }
    
    public void SendGridCopy(SudokuGrid9x9 copy, bool contradictedGrid)
    {
        OnSendGridCopy?.Invoke(copy, contradictedGrid);
    }

    public void HintFound(TileIndex hintIndex)
    {
        OnHintFound?.Invoke(hintIndex);
    }
}
