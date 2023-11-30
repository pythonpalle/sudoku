using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreationManager : MonoBehaviour
{
    [SerializeField] private GridPort gridPort;
    private WFCGridSolver _gridSolver = new WFCGridSolver(PuzzleDifficulty.Extreme);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryCreatePuzzle();
    }

    public void TryCreatePuzzle()
    {
        gridPort.RequestGrid();

        bool solveable = _gridSolver.HasOneSolution(gridPort.grid);
        if (!solveable)
        {
            bool multipleSolutions = _gridSolver.SolutionsState == SolutionsState.Multiple;
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
        
        bool humanlySolveable = _gridSolver.HumanlySolvable(gridPort.grid, out PuzzleDifficulty difficulty);
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
}
