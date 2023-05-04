using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private SudokuGenerator9x9 generator;
    
    void Start()
    {
        grid = new SudokuGrid9x9();
        generator = new SudokuGenerator9x9(grid);
    }

    public void GenerateFullGrid()
    {
        int iterations = 0;
        while (!generator.GenerationCompleted)
        {
            Generate();
            PrintGrid();

            iterations++;

            if (iterations > 100)
            {
                Debug.LogError("Maximum iterations reached, couldn't generate grid.");
                break;
            }
        }
    }

    private void Generate()
    {
        generator.Generate();
    }

    private void PrintGrid()
    {
        grid.PrintGrid();
    }
}
