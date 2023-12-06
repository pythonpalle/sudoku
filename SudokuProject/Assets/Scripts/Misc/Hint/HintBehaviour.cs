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
    private bool hintButtonIsFlashing;

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
            OnHintButtonClickedWithContradiction();
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

    private void OnHintButtonClickedWithContradiction()
    {
        if (!hintButtonIsFlashing)
            StartCoroutine(FlashHintButton());
        
        Debug.Log("Can't give hint if grid has contradiction");
    }

    private IEnumerator FlashHintButton()
    {
        hintButtonIsFlashing = true;

        int totalNumberOfFlashes = 2;
        float timeBetweenFlashes = 0.1f;

        for (int flashCounter = 0; flashCounter < totalNumberOfFlashes; flashCounter++)
        {
            yield return new WaitForSeconds(timeBetweenFlashes);
            hintButton.image.color = Color.black;
            yield return new WaitForSeconds(timeBetweenFlashes);
            hintButton.image.color = invalidHintColor.Color; 
        }
        
        if (!gridPort.gridContradicted)
            hintButton.image.color = selectColor.Color;

        hintButtonIsFlashing = false;
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
