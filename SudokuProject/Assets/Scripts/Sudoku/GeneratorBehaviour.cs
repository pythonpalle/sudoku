using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGrid9x9 grid;
    private SudokuGenerator9x9 generator;

    private const int GENERATION_ITERATION_LIMIT = 250;
    [SerializeField] private bool makeSymmetric;
    
    void Start()
    {
        grid = new SudokuGrid9x9();
        generator = new SudokuGenerator9x9(grid);
    }

    public void GenerateFullGrid()
    {
        Start();
        
        int iterations = 0;
        while (!generator.GenerationCompleted)
        {
            Generate();
            PrintGrid();

            iterations++;

            if (iterations >= GENERATION_ITERATION_LIMIT)
            {
                Debug.LogError("Maximum iterations reached, couldn't generate grid.");
                break;
            }
        }
    }

    private void Generate()
    {
        generator.Generate(makeSymmetric);
    }

    private void PrintGrid()
    {
        grid.PrintGrid();
    }
}
