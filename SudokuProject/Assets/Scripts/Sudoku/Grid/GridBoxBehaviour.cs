using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoxBehaviour : MonoBehaviour
{
    public int number;

    [SerializeField] private List<TileBehaviour> tiles;

    public void Setup(int number, RectTransform tileAnimationParent)
    {
        this.number = number;

        int startRow = number / 3 * 3;
        int startCol = number % 3 * 3;

        int i = 0;
        for (int deltaRow = 0; deltaRow < 3; deltaRow++)
        {
            for (int deltaCol = 0; deltaCol < 3; deltaCol++)
            {
                int row = startRow + deltaRow;
                int col = startCol + deltaCol;

                tiles[i].SetIndex(row, col);
                tiles[i].SetAnimationParent(tileAnimationParent);
                
                i++;
            }
        }
    }
}