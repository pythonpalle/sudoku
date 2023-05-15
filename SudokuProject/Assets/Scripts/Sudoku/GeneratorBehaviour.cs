using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGenerator9x9 generator;
    [SerializeField] private DifficultyObject _difficultyObject;

    void Awake()
    {
        generator = new SudokuGenerator9x9(_difficultyObject.Difficulty);
    }

    private void Start()
    {
        Invoke("GenerateFullGrid", 0.01f); 
    }

    public void GenerateFullGrid()
    {
        generator.Generate(_difficultyObject.Difficulty);
    }
}
