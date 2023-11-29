using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/HintObject")]
public class HintObject : ScriptableObject
{
    public UnityAction<TileIndex> OnHintFound;

    public void HintFound(TileIndex hintIndex)
    {
        OnHintFound?.Invoke(hintIndex);
    }
}