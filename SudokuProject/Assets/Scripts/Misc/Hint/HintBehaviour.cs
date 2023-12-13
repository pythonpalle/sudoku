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
    [SerializeField] private ColorObject noSolveColor;
    [SerializeField] private ColorObject multipleSolutionColor;
    [SerializeField] private ColorObject completeColor;
    
    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    private bool hintButtonIsFlashing;

    private GridStatus _status;

    private void OnEnable()
    {
        hintButton.onClick.AddListener(OnHintButtonClicked);
        gridPort.OnStatusChange += OnStatusChange;
    }
    

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(OnHintButtonClicked);
        gridPort.OnStatusChange -= OnStatusChange;
    }

    private void OnStatusChange(GridStatus status)
    {
        this._status = status;

        switch (status)
        {
            case GridStatus.Solved:
                SetColor(completeColor.Color);
                break;
            
            case GridStatus.OneSolution:
                SetColor(selectColor.Color);
                break;
            
            case GridStatus.MultipleSolutions:
                SetColor(multipleSolutionColor.Color);
                break;
            
            case GridStatus.Unsolvable:
                SetColor(noSolveColor.Color);
                break;
        }
    }

    private void SetColor(Color color)
    {
        hintButton.image.color = color;
    }

    private void OnHintButtonClicked()
    {
        gridPort.RequestGrid();
        
        switch (_status)
        {
            case GridStatus.Solved:
                Debug.Log("Grid solved already!");
                return;
            
            case GridStatus.OneSolution:
                // check to see if progression can be made with human methods
                if (TryFindProgression(gridPort.grid, out TileIndex hintIndex))
                {
                    hintObject.HintFound(hintIndex);
                }
                else
                {
                    DisplayWarning("Solver could only progress using brute force.");
                }
                break;
            
            case GridStatus.MultipleSolutions:
                DisplayWarning("The puzzle has multiple solutions.");
                return;
            
            case GridStatus.Unsolvable:
                DisplayWarning("The puzzle is not solvable from this state.");
                return;
        }
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
            hintButton.image.color = noSolveColor.Color; 
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
