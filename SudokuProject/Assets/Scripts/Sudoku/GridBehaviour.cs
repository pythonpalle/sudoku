using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private TileBehaviour[,] tileBehaviours = new TileBehaviour[9,9];

    [SerializeField] private List<GridBoxBehaviour> boxes;
    
    private void OnEnable()
    {
        EventManager.OnGridGenerated += OnGridGenerated;
        EventManager.OnTileIndexSet += OnTileIndexSet;
    }
    
    private void OnDisable()
    {
        EventManager.OnGridGenerated -= OnGridGenerated;
        EventManager.OnTileIndexSet -= OnTileIndexSet;
    }

    private void Start()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].Setup(i);
        }
    }

    private void OnGridGenerated(SudokuGrid9x9 generatedGrid)
    {
        grid = generatedGrid;
    }
    
    private void OnTileIndexSet(int row,  int col, TileBehaviour tileBehaviour)
    {
        tileBehaviours[row, col] = tileBehaviour;
    }
}
