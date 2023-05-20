using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    [SerializeField] private DifficultyObject difficultyObject;

    [Header("Buttons")] 
    [SerializeField] private Button simpleButton;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    private void OnEnable()
    {
        simpleButton.onClick.AddListener(OnSimpleButton);
        easyButton.onClick.AddListener(OnEasyButton);
        mediumButton.onClick.AddListener(OnMediumButton);
        hardButton.onClick.AddListener(OnHardButton);
    }
    
    private void OnDisable()
    {
        simpleButton.onClick.RemoveListener(OnSimpleButton);
        easyButton.onClick.RemoveListener(OnEasyButton);
        mediumButton.onClick.RemoveListener(OnMediumButton);
        hardButton.onClick.RemoveListener(OnHardButton);
    }
    
    private void OnSimpleButton()
    {
        SetDifficultyAndResetPuzzle(PuzzleDifficulty.Simple);
    }

    private void OnEasyButton()
    {
        SetDifficultyAndResetPuzzle(PuzzleDifficulty.Easy);
    }

    private void OnMediumButton()
    {
        SetDifficultyAndResetPuzzle(PuzzleDifficulty.Medium);
    }
    
    private void OnHardButton()
    {
        SetDifficultyAndResetPuzzle(PuzzleDifficulty.Hard);
    }
    
    private void SetDifficultyAndResetPuzzle(PuzzleDifficulty difficulty)
    {
        difficultyObject.Difficulty = difficulty;
        LoadPuzzleScene();
    }

    private void LoadPuzzleScene()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
