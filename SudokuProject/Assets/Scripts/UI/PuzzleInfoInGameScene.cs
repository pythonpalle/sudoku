using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleSelect;
using Saving;
using TMPro;
using UnityEngine;

public class PuzzleInfoInGameScene : MonoBehaviour
{
    [SerializeField] private PuzzleSelectPort _puzzleSelectPort;
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SaveRequestPort _saveRequestPort;
    
    private void OnEnable()
    {
        SaveManager.OnPuzzleSaveCreated += OnSuccessfulSave;

        switch (generatorPort.GenerationType)
        {
            case GridGenerationType.loaded:
                SetNameText();
                break;
            
            case GridGenerationType.random:
                SetEmptyName();
                break;
        }
    }

    private void OnDisable()
    {
        SaveManager.OnPuzzleSaveCreated -= OnSuccessfulSave;
    }

    private void OnSuccessfulSave()
    {
        if (_saveRequestPort.Location == SaveRequestLocation.SaveButton)
            SetNameText();
    }

    private void SetEmptyName()
    {
        nameText.text = "";
    }

    private void SetNameText()
    {
        nameText.text = SaveManager.currentPuzzle.name;
    }
}
