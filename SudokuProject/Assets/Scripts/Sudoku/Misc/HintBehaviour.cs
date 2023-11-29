using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HintBehaviour : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private HintObject hintObject;
    [SerializeField] private GridPort gridPort;
    
    [Header("Button")]
    [SerializeField] private Button hintButton;

    [Header("Color")] 
    [SerializeField] private ColorObject selectColor;
    [SerializeField] private ColorObject invalidHintColor;
    
    [Header("Popup")] 
    [SerializeField] private PopupWindow _popupWindow;

    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private void OnEnable()
    {
        hintButton.onClick.AddListener(OnHintButtonClicked);

        gridPort.OnContradictionStatusUpdate += OnContradictionStatusUpdate;
    }
    

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(OnHintButtonClicked);
        
        gridPort.OnContradictionStatusUpdate -= OnContradictionStatusUpdate;
    }

    private void OnContradictionStatusUpdate(bool contradiction)
    {
        hintButton.image.color = contradiction ? invalidHintColor.Color : selectColor.Color;
    }

    private void OnHintButtonClicked()
    {
        gridPort.RequestGrid();
        
        if (gridPort.gridContradicted)
        {
            Debug.Log("Can't give hint if grid has contradiction");
            return;
        }

        // TODO: Custom error texts depending on if 
        // a) not humanly solveable
        // b) no solutions
        // c) multiple solutions
        
        if (!_solver.HasOneSolution(gridPort.grid))
        {
            Debug.Log("Puzzle not solveable");
            _popupWindow.PopUp();
            return;
        }
        
        if (TryFindHint(gridPort.grid, out TileIndex hintIndex))
        {
            hintObject.HintFound(hintIndex);
        }
        else
        {
            _popupWindow.PopUp();
        }
    }

    private bool TryFindHint(SudokuGrid9x9 gridCopy, out TileIndex tileIndex)
    {
        return _solver.TryFindProgression(gridCopy, PuzzleDifficulty.Extreme, out tileIndex);
    }
}
