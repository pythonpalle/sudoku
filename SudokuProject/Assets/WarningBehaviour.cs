using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBehaviour : MonoBehaviour
{
    [SerializeField] private ExplanationText warningText;
    [SerializeField] private GridPort _gridPort;

    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private void OnEnable()
    {
        EventManager.OnNewCommand += UpdateSolveableState;

        EventManager.OnRedo += UpdateSolveableState;
        EventManager.OnUndo += UpdateSolveableState;
    }
    
    private void OnDisable()
    {
        EventManager.OnNewCommand -= UpdateSolveableState;
        
        EventManager.OnRedo -= UpdateSolveableState;
        EventManager.OnUndo -= UpdateSolveableState;
    }

    private void UpdateSolveableState()
    {
        _gridPort.RequestGrid();
        bool solveable = _solver.HasOneSolution(_gridPort.grid);

        if (solveable)
        {
            warningText.gameObject.SetActive(false);
        }
        else
        {
            warningText.gameObject.SetActive(true);
        }
    }

    private void UpdateSolveableState(SudokuEntry entry)
    {
        UpdateSolveableState();
    }
}
