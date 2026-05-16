using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleSelect;
using Saving;
using TMPro;
using UnityEngine;

public class PuzzleInfoInGameScene : MonoBehaviour, ILoadPuzzleData
{
    [SerializeField] private PuzzleSelectPort _puzzleSelectPort;
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private SaveRequestPort _saveRequestPort;
    
    private void OnEnable()
    {
        SaveManager.AddLoadDataListener(this);
        
        SaveManager.OnPuzzleSaveCreated += OnSuccessfulSave;

        switch (generatorPort.GenerationType)
        {
            case GridGenerationType.loaded:
               // SetNameText();
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
        if (SaveManager.currentPuzzle == null)
        {
            Debug.LogError("Save manager ha no current puzzle");
            return;
        }
        
        nameText.text = SaveManager.currentPuzzle.name;
    }
    
    private void SetNameText(PuzzleDataHolder dataHolder)
    {
        if (dataHolder != null && !string.IsNullOrEmpty(dataHolder.name))
            nameText.text = dataHolder.name;
    }

    public void LoadFromSaveData(PuzzleDataHolder dataHolder)
    {
        SetNameText(dataHolder);
    }
}
