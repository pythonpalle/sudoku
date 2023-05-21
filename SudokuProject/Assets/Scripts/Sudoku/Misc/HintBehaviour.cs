using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HintBehaviour : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private HintObject hintObject;
    [SerializeField] private DifficultyObject difficultyObject;
    
    [Header("Button")]
    [SerializeField] private Button hintButton;

    private WFCGridSolver _solver;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        _solver = new WFCGridSolver(difficultyObject.Difficulty);
        
        hintButton.onClick.AddListener(OnHintButtonClicked);

        hintObject.OnSendGridCopy += OnSendGridCopy;
    }
    

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(OnHintButtonClicked);
        
        hintObject.OnSendGridCopy -= OnSendGridCopy;
    }

    private void OnHintButtonClicked()
    {
        hintObject.RequestGrid();
    }
    
    private void OnSendGridCopy(SudokuGrid9x9 gridCopy, bool contradiction)
    {
        if (contradiction)
        {
            Debug.Log("Can't find hint; grid contradicted.");
            return;
        }
        
        if (TryFindHint(gridCopy, out TileIndex hintIndex))
        {
            hintObject.HintFound(hintIndex);
        }
        
    }

    private bool TryFindHint(SudokuGrid9x9 gridCopy, out TileIndex tileIndex)
    {
        return _solver.TryFindProgression(gridCopy, difficultyObject.Difficulty, out tileIndex);
    }
}
