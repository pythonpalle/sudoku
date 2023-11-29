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

    [Header("Color")] 
    [SerializeField] private ColorObject selectColor;
    [SerializeField] private ColorObject invalidHintColor;
    
    [Header("Popup")] 
    [SerializeField] private PopupWindow _popupWindow;

    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private void OnEnable()
    {
        hintButton.onClick.AddListener(OnHintButtonClicked);

        hintObject.OnSendGridCopy += OnSendGridCopy;
        hintObject.OnContradictionStatusUpdate += OnContradictionStatusUpdate;
    }
    

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(OnHintButtonClicked);
        
        hintObject.OnSendGridCopy -= OnSendGridCopy;
        hintObject.OnContradictionStatusUpdate -= OnContradictionStatusUpdate;
    }

    private void OnContradictionStatusUpdate(bool contradiction)
    {
        hintButton.image.color = contradiction ? invalidHintColor.Color : selectColor.Color;
    }

    private void OnHintButtonClicked()
    {
        hintObject.RequestGrid();
    }
    
    private void OnSendGridCopy(SudokuGrid9x9 gridCopy)
    {
        if (TryFindHint(gridCopy, out TileIndex hintIndex))
        {
            hintObject.HintFound(hintIndex);
        }
        else
        {
            _popupWindow.PopUp();
        }
    }

    private bool TryFindHint(SudokuGrid9x9 gridCopy, out TileIndex tileIndex)
    {
        return _solver.TryFindProgression(gridCopy, PuzzleDifficulty.Extreme, out tileIndex);
    }
}
