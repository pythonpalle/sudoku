using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/ColorObject")]
public class ColorObject : ScriptableObject
{
    [SerializeField] private Color _color;
    public Color Color => _color;

    public UnityAction<Color> OnColorChange;

    private void OnValidate()
    {
        OnColorChange?.Invoke(_color);
    }
}