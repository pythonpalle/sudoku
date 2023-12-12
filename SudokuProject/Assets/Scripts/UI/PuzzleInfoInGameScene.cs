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
    
    private void OnEnable()
    {
        SaveManager.OnSuccessfulSave += OnSuccessfulSave;

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
        SaveManager.OnSuccessfulSave -= OnSuccessfulSave;
    }

    private void OnSuccessfulSave(SaveRequestLocation loc)
    {
        if (generatorPort.GenerationType == GridGenerationType.random)
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
