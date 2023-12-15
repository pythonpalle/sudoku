using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/Color Split Holder")]
public class ColorSplitHolder : ScriptableObject
{
    public Texture2D[] ColorSplits;
}

[System.Serializable]
public class ColorSplit
{
    public int splitCount;
    public Texture2D Texture;
}