using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/TileColors")]
public class TileColors : ScriptableObject
{
    [SerializeField] private List<Color> _colors;
    public List<Color> Colors => _colors;
}