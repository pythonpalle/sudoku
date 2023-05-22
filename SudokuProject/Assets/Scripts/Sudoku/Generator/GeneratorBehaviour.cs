using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class GeneratorBehaviour : MonoBehaviour
{
    private SudokuGenerator9x9 generator;
    [SerializeField] private DifficultyObject _difficultyObject;
    [SerializeField] private LoadGrid loadGrid;

    void Awake()
    {
        generator = new SudokuGenerator9x9(_difficultyObject.Difficulty);
    }

    private void Start()
    {
        StartCoroutine(AnimateGrid());
        Invoke("GenerateFullGrid", 0.01f);
    }

    public void GenerateFullGrid()
    {
        StartCoroutine(generator.GenerateWithRoutine(_difficultyObject.Difficulty));
    }
    

    private IEnumerator AnimateGrid()
    {
        loadGrid.gameObject.SetActive(true);

        int shuffleCount = 0;
        int minShuffles = 3;
        float shuffleWait = 0.1f;
        while (!generator.Finished || shuffleCount < minShuffles)
        {
            loadGrid.Shuffle(_difficultyObject.Difficulty);
            yield return new WaitForSeconds(shuffleWait);
            shuffleCount++;
        }
        loadGrid.Shuffle(_difficultyObject.Difficulty);
        yield return new WaitForSeconds(shuffleWait);
        
        loadGrid.gameObject.SetActive(false);
    }
}
