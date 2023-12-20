using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoader : MonoBehaviour
{
    [SerializeField] private int saveNumber;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;
    [SerializeField] ScenePort _scenePort;
    [SerializeField] ColorObject deselectColor;
    [SerializeField] ColorObject selectColor;

    private void OnEnable()
    {
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user, true))
        {
            text.text = $"Save {saveNumber + 1} - {user.GetTotalPuzzleCount()} puzzles.";

            bool isCurrentSaveNumber = SaveManager.currentSaveNumber == saveNumber;

            float alpha = isCurrentSaveNumber ? 1f : 0.5f;
            background.color = new Color(background.color.r, background.color.g, background.color.b, alpha);
            
            //background.color = isCurrentSaveNumber ? selectColor.Color : deselectColor.Color;
        }
    }

    public void LoadSave()
    {
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData _, false))
        {
            _scenePort.CallLoadPuzzleSelectScene();
        }
    }
}
