using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        
        // don't give hints if the grid is contradicted
        if (gridPort.gridContradicted)
        {
            Debug.Log("Can't give hint if grid has contradiction");
            return;
        }

        // check if valid grid (only one solution)
        if (_solver.HasOneSolution(gridPort.grid))
        {
            // check to see if progression can be made with human methods
            if (TryFindProgression(gridPort.grid, out TileIndex hintIndex))
            {
                hintObject.HintFound(hintIndex);
            }
            else
            {
                DisplayWarning("Solver could only progress using brute force.");
            }
        }
        else
        {
            switch (_solver.SolutionsState)
            {
                case SolutionsState.Multiple:
                    DisplayWarning("The puzzle has multiple solutions.");
                    return;
            
                case SolutionsState.None:
                    DisplayWarning("The puzzle is not solveable from this state.");
                    return;
            }
        }
    }

    private void DisplayWarning(string warningMessage)
    {
        hintObject.DisplayWarningPopup(warningMessage);
    }

    private bool TryFindProgression(SudokuGrid9x9 gridCopy, out TileIndex tileIndex)
    {
        return _solver.TryFindProgression(gridCopy, out tileIndex);
    }
}
