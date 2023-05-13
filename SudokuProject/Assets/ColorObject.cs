using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/ColorObject")]
public class ColorObject : ScriptableObject
{
    [SerializeField] private Color _color;
    public Color Color => _color;
}