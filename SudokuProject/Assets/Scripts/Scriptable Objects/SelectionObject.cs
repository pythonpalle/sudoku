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
    // move keys
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
    
    // selection keys
    [Header("Multi Select Key")]
    [SerializeField] private KeyCode multiKey1 = KeyCode.LeftControl;
    [SerializeField] private KeyCode multiKey2 = KeyCode.RightControl;
    
    [Header("Center Key")]
    [SerializeField] private KeyCode centerKey1 = KeyCode.LeftControl;
    [SerializeField] private KeyCode centerKey2 = KeyCode.RightControl;
  
    [Header("Corner Key")]
    [SerializeField] private KeyCode cornerKey1 = KeyCode.LeftShift;
    [SerializeField] private KeyCode cornerKey2 = KeyCode.RightShift;
    
    [Header("All Key")]
    [SerializeField] private KeyCode allKey = KeyCode.A;

    // key press checks
    // move keys. Excludes keypad arrows.
    public bool rightKeyIsPressed => (Input.GetKeyDown(rightKey1) || Input.GetKeyDown(rightKey2));
    public bool leftKeyIsPressed => (Input.GetKeyDown(leftKey1) || Input.GetKeyDown(leftKey2));
    public bool upKeyIsPressed => (Input.GetKeyDown(upKey1) || Input.GetKeyDown(upKey2)) ;
    public bool downKeyIsPressed => (Input.GetKeyDown(downKey1) || Input.GetKeyDown(downKey2)) ;
    
    // public bool rightKeyIsPressed => (Input.GetKeyDown(rightKey2));
    // public bool leftKeyIsPressed => Input.GetKeyDown(leftKey2);
    // public bool upKeyIsPressed => Input.GetKeyDown(upKey2) ;
    // public bool downKeyIsPressed => (Input.GetKeyDown(downKey2)) ;
    
    
    // multi keys
    public bool multiSelectKeyIsPressed => Input.GetKey(multiKey1) || Input.GetKey(multiKey2);
    
    // selection keys
    public bool centerSelectKeyIsPressed => Input.GetKeyDown(centerKey1);// || Input.GetKeyDown(centerKey2);
    public bool centerSelectKeyIsReleased => Input.GetKeyUp(centerKey1);// || Input.GetKeyUp(centerKey2);

    public bool cornerSelectKeyIsPressed => Input.GetKeyDown(cornerKey1);// || Input.GetKeyDown(cornerKey2);
    public bool cornerSelectKeyIsReleased => Input.GetKeyUp(cornerKey1);// || Input.GetKeyUp(cornerKey2);

    private bool centerKeyHeld => Input.GetKey(centerKey1) || Input.GetKey(centerKey2);
    private bool cornerKeyHeld => Input.GetKey(cornerKey1) || Input.GetKey(cornerKey2);
    
    public bool colorSelectKeyIsPressed => (centerKeyHeld && cornerSelectKeyIsPressed)
                                            || (cornerKeyHeld && centerSelectKeyIsPressed);
    public bool colorSelectKeyIsReleased => (centerKeyHeld && cornerSelectKeyIsReleased)
                                            || (cornerKeyHeld && centerSelectKeyIsReleased);

    public bool colorSelectKeyIsHeld => centerKeyHeld && cornerKeyHeld;

    public bool AllCellsKeyIsPressed => centerKeyHeld && Input.GetKeyDown(allKey);


    
    // fields
    [SerializeField] private SelectionMode selectionMode = SelectionMode.None;
    public List<TileBehaviour> SelectedTiles { get; private set; } = new List<TileBehaviour>();

    public bool IsSelecting => (selectionMode == SelectionMode.Selecting);
    public bool IsDeselecting => (selectionMode == SelectionMode.Deselecting);

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


    public void TryAdd(TileBehaviour tile)
    {
        if (!SelectedTiles.Contains(tile))
        {
            SelectedTiles.Add(tile);
        }
    }

    public void TryRemove(TileBehaviour tile)
    {
        if (SelectedTiles.Contains(tile))
        {
            SelectedTiles.Remove(tile);
        }
    }

    public TileBehaviour LastSelected()
    {
        return SelectedTiles[^1];
    }
}
