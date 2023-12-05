using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBehaviour : MonoBehaviour, IHasCommand
{
    [SerializeField] private ExplanationText warningText;
    [SerializeField] private GridPort _gridPort;

    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private int stateCounter;
    private List<bool> solveableHistory = new List<bool>();
    
    private void OnEnable()
    {
        EventManager.OnNewCommand += OnNewCommand;

        EventManager.OnRedo += OnRedo;
        EventManager.OnUndo += OnUndo;
    }
    
    private void OnDisable()
    {
        EventManager.OnNewCommand -= OnNewCommand;
        
        EventManager.OnRedo -= OnRedo;
        EventManager.OnUndo -= OnUndo;
    }

    public void OnNewCommand(SudokuEntry entry)
    {
        while (solveableHistory.Count > stateCounter)
        {
            solveableHistory.RemoveAt(solveableHistory.Count-1);
        }
        
        _gridPort.RequestGrid();

        bool solveable = true;
        if (_gridPort.gridContradicted || !_solver.HasOneSolution(_gridPort.grid))
        {
            solveable = false;
        }
        
        solveableHistory.Add(solveable);
        stateCounter++;
        
        UpdateSolvable(solveable);
    }

    void UpdateSolvable(bool solveable)
    {
        if (solveable)
        {
            warningText.gameObject.SetActive(false);
        }
        else
        {
            warningText.gameObject.SetActive(true);
        }
    }
    
    private void OnUndo()
    {
        ChangeState(true);
    }
    
    private void OnRedo()
    {
        ChangeState(false);
    }

    
    private void ChangeState(bool undo)
    {
        if (undo)
            stateCounter--;
        else
            stateCounter++;
        
        if (undo && stateCounter <= 0)
        {
            stateCounter = 1;
            return;
        }
        
        if (!undo && stateCounter > solveableHistory.Count)
        {
            stateCounter --;
            return;
        }
        
        UpdateSolvable(solveableHistory[stateCounter-1]);
    }
}
