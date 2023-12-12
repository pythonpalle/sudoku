using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CompletionPopupActivator : MonoBehaviour
{
    public UnityEvent OnComplete;
    private int completions = 0;
    
    [SerializeField] private PopupData _popupData;
    [SerializeField] private ScenePort _scenePort;

    private void OnEnable()
    {
        EventManager.OnPuzzleComplete += OnPuzzleComplete;
    }
    
    private void OnDisable()
    {
        EventManager.OnPuzzleComplete -= OnPuzzleComplete;
    }

    private void OnPuzzleComplete()
    {
        OnComplete?.Invoke();
        completions++;

        if (completions > 1)
        {
            _popupData.explanation = "wow, I guess you solved this one again...";
        }

        _popupData.cancelButtonData.action = GoBackToSelect;
        EventManager.OnDisplayConfirmPopup(_popupData);
    }

    private void GoBackToSelect()
    {
        _scenePort.CallLoadPuzzleSelectScene();
    }
}
