using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreationManager : MonoBehaviour
{
    [SerializeField] private CreationObject creationObject;

    private SudokuGrid9x9 grid;
    private WFCGridSolver _gridSolver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    private void OnEnable()
    {
        creationObject.OnSendGridCopy += OnSendGridCopy;
    }
    
    private void OnDisable() 
    {
        creationObject.OnSendGridCopy -= OnSendGridCopy;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryCreatePuzzle();
    }

    public void TryCreatePuzzle()
    {
        creationObject.RequestGrid();
        
        bool solveable = _gridSolver.HasOneSolution(grid);
        if (!solveable)
        {
            bool multipleSolutions = _gridSolver.SolutionCount > 1;
            if (multipleSolutions)
            {
                Debug.LogWarning("the created puzzle has multiple solutions.");
            }
            else
            {
                Debug.LogWarning("the created puzzle has no solutions.");
            }

            return;
        }

        bool humanlySolveable = _gridSolver.HumanlySolvable(grid, out PuzzleDifficulty difficulty);
        if (humanlySolveable)
        {
            Debug.Log("Puzzle looks ok too me, dude!");
            Debug.Log($"It is rated difficulty {difficulty}");
        }
        else
        {
            Debug.LogWarning("the created puzzle may be very hard!");
            return;
        }
        
    }

    private void OnSendGridCopy(SudokuGrid9x9 grid)
    {
        this.grid = grid;
    }
}
