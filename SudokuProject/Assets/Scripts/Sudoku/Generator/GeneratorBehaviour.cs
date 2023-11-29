using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGenerator9x9 generator;
    private SudokuGenerator9x9 fakeGenerator;
    
    [SerializeField] private DifficultyObject _difficultyObject;
    [SerializeField] private LoadGrid loadGrid;
    
    [Header("Grid Generation Settings")] 
    [SerializeField] private bool createEmptyGrid;

    void Awake()
    {
        generator = new SudokuGenerator9x9(_difficultyObject.Difficulty);
        fakeGenerator = new SudokuGenerator9x9(_difficultyObject.Difficulty);
    }

    private void Start()
    {
        if (createEmptyGrid)
        {
            Invoke("GenerateEmptyGrid", 0.01f);
        }
        else
        {
            StartCoroutine(AnimateGrid());
            Invoke("GenerateFullGrid", 0.01f);
        }
    }
    
    public void GenerateEmptyGrid()
    {
        generator.GenerateEmptyGrid();
    }

    public void GenerateFullGrid()
    {
        StartCoroutine(generator.GenerateWithRoutine(_difficultyObject.Difficulty));
    }
    

    private IEnumerator AnimateGrid()
    {
        SudokuGrid9x9 grid = fakeGenerator.GetRandomCompleteGrid();
        
        loadGrid.gameObject.SetActive(true);

        int shuffleCount = 0;
        int minShuffles = 20;
        float shuffleWait = 0.1f;
        
        while (!generator.Finished || shuffleCount < minShuffles)
        {
            loadGrid.Shuffle(grid, _difficultyObject.Difficulty);
            yield return new WaitForSeconds(shuffleWait);
            shuffleCount++;
        }
        loadGrid.Shuffle(grid, _difficultyObject.Difficulty);
        yield return new WaitForSeconds(0.5f);

        loadGrid.gameObject.SetActive(false);
    }
}
