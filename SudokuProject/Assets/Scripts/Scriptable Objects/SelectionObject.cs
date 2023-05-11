using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/SelectionObject")]
public class SelectionObject : ScriptableObject
{
    public bool IsSelecting { get; private set; }  = false;

    public UnityAction<int, int> OnRequestTile;
    public UnityAction<TileBehaviour> OnSendTileReference;

    public void SendTileRequest(int row, int col)
    {
        OnRequestTile?.Invoke(row, col);
    }

    public void SendTileReference(TileBehaviour tileReference)
    {
        OnSendTileReference?.Invoke(tileReference);
    }

    public void SetSelect(bool value)
    {
        IsSelecting = value;
    }
}
