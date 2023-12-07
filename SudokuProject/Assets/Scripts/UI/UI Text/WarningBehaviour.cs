using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningBehaviour : MonoBehaviour, IHasCommand
{
    [SerializeField] private ExplanationText warningText;
    [SerializeField] private GridPort _gridPort;
    
    [Header("Colors")]
    [SerializeField] private Image warningTriangle;
    [SerializeField] private Color noSolutionColor;
    [SerializeField] private Color multiSolutionColor;

    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private int stateCounter;
    private List<SolutionsState> solveableHistory = new List<SolutionsState>();
    
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

        SolutionsState state = SolutionsState.Single;
        if (_gridPort.gridContradicted)
        {
            state = SolutionsState.None;
        }
        else
        {
            _solver.HasOneSolution(_gridPort.grid);
            state = _solver.SolutionsState;
        }

        solveableHistory.Add(state);
        stateCounter++;
        
        UpdateSolvable(state);
    }

    void UpdateSolvable(SolutionsState state)
    {
        warningText.gameObject.SetActive(true);

        switch (state)
        {
            case SolutionsState.Single:
                warningText.gameObject.SetActive(false);
                warningText.SetText("Grid can be solved!");
                break;
            
            case SolutionsState.None:
                warningText.SetText("Grid has no solutions!");
                warningTriangle.color = noSolutionColor;
                break;
            
            case SolutionsState.Multiple:
                warningText.SetText("Grid has several solutions!");
                warningTriangle.color = multiSolutionColor;
                break;
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
