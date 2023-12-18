using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CompletionPopupActivator : MonoBehaviour
{
    private int completions = 0;
    
    [SerializeField] private PopupData _popupData;
    [SerializeField] private ScenePort _scenePort;
    [SerializeField] private GeneratorPort generatorPort;

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
        if (SaveManager.TrySave(SaveRequestLocation.ExitGameButton, generatorPort.GenerationType))
        {
            _scenePort.OnCallLoadPuzzleSelectScene();
        }
    }
}
