using TMPro;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public int row;
    public int col;
    public int number;

    [SerializeField] private TextMeshProUGUI numberText;

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
        
        EventManager.SetTileIndex(row, col, this);
    }

    public void SetNumber(int number)
    {
        this.number = number;

        numberText.text = number > 0 ? number.ToString() : string.Empty;
    }
}