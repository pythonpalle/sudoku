using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/SelectionObject")]
public class SelectionObject : ScriptableObject
{
    // TODO: Flytta hit rightKeyIsPressed etc hit från selection manager
    
    public bool IsSelecting { get; private set; }  = false;

    public List<TileBehaviour> SelectedTiles { get; private set; } = new List<TileBehaviour>();
    public bool HasSelectedTiles => SelectedTiles.Count > 0;

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

    public void ClearSelectedTiles()
    {
        SelectedTiles = new List<TileBehaviour>();
    }
}
