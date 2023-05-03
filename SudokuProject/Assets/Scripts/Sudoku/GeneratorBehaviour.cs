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

    public void Generate()
    {
        generator.Generate();
    }

    public void PrintGrid()
    {
        grid.PrintGrid();
    }
}
