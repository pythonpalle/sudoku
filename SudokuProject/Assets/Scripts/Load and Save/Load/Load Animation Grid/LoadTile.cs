using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadTile : MonoBehaviour
{
    public TextMeshProUGUI TileText;

    public int row;
    public int col;

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
