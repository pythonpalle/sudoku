using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGenerator9x9 generator;

    void Awake()
    {
        generator = new SudokuGenerator9x9();
    }

    public void GenerateFullGrid()
    {
        Awake();        
        generator.Generate();
    }
}
