using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningBehaviour : MonoBehaviour
{
    [SerializeField] private ExplanationText warningText;
    [SerializeField] private GridPort _gridPort;
    
    [Header("Colors")]
    [SerializeField] private Image warningTriangle;
    [SerializeField] private ColorObject noSolveColor;
    [SerializeField] private ColorObject multipleSolutionColor;
    [SerializeField] private ColorObject completeColor;

    private void OnEnable()
    {
        _gridPort.OnStatusChange += OnStatusChange;
    }
    
    private void OnDisable()
    {
        _gridPort.OnStatusChange -= OnStatusChange;
    }

    private void OnStatusChange(GridStatus status)
    {
        UpdateSolvable(status);
    }

    void UpdateSolvable(GridStatus status)
    {
        warningText.gameObject.SetActive(true);

        switch (status)
        {
            case GridStatus.OneSolution:
                warningText.gameObject.SetActive(false);
                warningText.SetText("Grid can be solved!");
                break;
            
            case GridStatus.Unsolvable:
                warningText.SetText("Grid has no solutions!");
                warningTriangle.color = noSolveColor.Color;
                break;
            
            case GridStatus.MultipleSolutions:
                warningText.SetText("Grid has several solutions!");
                warningTriangle.color = multipleSolutionColor.Color;
                break;
            
            case GridStatus.Solved:
                warningText.SetText("Grid complete!");
                warningTriangle.color = completeColor.Color;
                break;
        }
    }
}
