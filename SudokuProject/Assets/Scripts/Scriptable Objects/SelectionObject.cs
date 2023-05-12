using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SelectionMode
{
    None,
    Selecting,
    Deselecting
}

[CreateAssetMenu(menuName = "Sudoku/SelectionObject")]
public class SelectionObject : ScriptableObject
{
    // keys
    [Header("Right Key")]
    [SerializeField] private KeyCode rightKey1 = KeyCode.RightArrow;
    [SerializeField] private KeyCode rightKey2 = KeyCode.D;
    
    [Header("Left Key")]
    [SerializeField] private KeyCode leftKey1 = KeyCode.LeftArrow;
    [SerializeField] private KeyCode leftKey2 = KeyCode.A;
    
    [Header("Up Key")]
    [SerializeField] private KeyCode upKey1 = KeyCode.UpArrow;
    [SerializeField] private KeyCode upKey2 = KeyCode.W;
    
    [Header("Down Key")]
    [SerializeField] private KeyCode downKey1 = KeyCode.DownArrow;
    [SerializeField] private KeyCode downKey2 = KeyCode.S;
    
    
    public bool rightKeyIsPressed => Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    public bool leftKeyIsPressed => Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    public bool upKeyIsPressed => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    public bool downKeyIsPressed => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

    public bool moveKeyIsPressed => rightKeyIsPressed || leftKeyIsPressed || upKeyIsPressed || downKeyIsPressed;
    public bool multiSelectKeyIsPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    public bool centerSelectKeyIsPressed => Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
    public bool centerSelectKeyIsReleased => Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl);

    public bool cornerSelectKeyIsPressed => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    public bool cornerSelectKeyIsReleased => Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
    
    // key detection

    [SerializeField] private SelectionMode selectionMode = SelectionMode.None;
    //
    //
    // public bool IsSelecting { get; private set; }  = false;
    // public bool IsDeselecting { get; private set; }  = false;

    public bool IsSelecting => (selectionMode == SelectionMode.Selecting);
    public bool IsDeselecting => (selectionMode == SelectionMode.Deselecting);

    //public List<TileBehaviour> SelectedTiles { get; private set; } = new List<TileBehaviour>();
    public List<TileBehaviour> SelectedTiles = new List<TileBehaviour>();
    public bool HasSelectedTiles => SelectedTiles.Count > 0;
    public bool SelectionKeyDown => Input.GetMouseButton(0);

    public UnityAction<int, int> OnRequestTile;
    public UnityAction<TileBehaviour> OnSendTileReference;
    
    public UnityAction OnDeselectAllTiles;

    public void SendTileRequest(int row, int col)
    {
        OnRequestTile?.Invoke(row, col);
    }

    public void SendTileReference(TileBehaviour tileReference)
    {
        OnSendTileReference?.Invoke(tileReference);
    }
    
    public void DeselectAllTiles()
    {
        SelectedTiles.Clear();
        OnDeselectAllTiles?.Invoke();
    }
    
    public void SetSelectionMode(SelectionMode mode)
    {
        selectionMode = mode;
    }

    public void ClearSelectedTiles()
    {
        SelectedTiles = new List<TileBehaviour>();
    }


    // public void SetDeselect(bool value)
    // {
    //     IsDeselecting = value;
    // }
}
