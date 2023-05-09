using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public int row;
    public int col;

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
        
        EventManager.SetTileIndex(row, col, this);
    }
}