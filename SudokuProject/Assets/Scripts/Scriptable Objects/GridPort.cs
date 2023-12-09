using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/GridPort")]
public class GridPort : ScriptableObject
{
    public SudokuGrid9x9 grid { get; private set; }
    public bool gridContradicted { get; private set; }

    public TileBehaviour[,] tileBehaviours { get; set; }

    public UnityAction<bool> OnContradictionStatusUpdate;
    public UnityAction OnRequestGrid;
    public UnityAction OnRequestTiles;

    public void RequestGrid()
    {
        OnRequestGrid?.Invoke();
    }
    
    public void RequestTiles()
    {
        OnRequestTiles?.Invoke();
    }

    public void SendGridCopy(SudokuGrid9x9 gridCopy, TileBehaviour[,] tileBehaviours)
    {
        grid = gridCopy;
        this.tileBehaviours = tileBehaviours;
    }
    
    public void UpdateContradictionStatus(bool gridHasContradiction)
    {
        OnContradictionStatusUpdate?.Invoke(gridHasContradiction);
        gridContradicted = gridHasContradiction;
    }

    public string GetGridAsSeed()
    {
        return grid.AsString();
    }

    public void Reset()
    {
        gridContradicted = false;
    }

    public List<TileBehaviour> GetTiles(List<int> commandDataTiles)
    {
        List<TileBehaviour> tiles = new List<TileBehaviour>();

        for (int i = 0; i < commandDataTiles.Count; i++)
        {
            int index = commandDataTiles[i];
            int row = index / 9;
            int col = index % 9;

            TileBehaviour tile = tileBehaviours[row, col];
            tiles.Add(tile);

            if (tile == null)
            {
                Debug.Log($"Row: {row}");
                Debug.Log($"Col: {col}");
                Debug.LogError("Null tile!");
            }
        }

        return tiles;
    }
}