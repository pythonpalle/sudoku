using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileLoader : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private int saveNumber;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;

    [Header("Colors")] 
    [SerializeField] private ColorObject selectColor;
    [SerializeField] private ColorObject deselectColor;
    
    [Header("Ports")]
    [SerializeField] ScenePort _scenePort;
    
    [Header("Removal")]
    [SerializeField] Button removeButton;
    [SerializeField] Image removeImage;
    [SerializeField] bool displayRemoval;

    private void OnEnable()
    {
        removeButton.gameObject.SetActive(displayRemoval);
        
        string savePrefixText = $"Save {saveNumber + 1} - ";

        bool isCurrentSaveNumber = SaveManager.currentSaveNumber == saveNumber;
        background.color = isCurrentSaveNumber ? selectColor.Color : deselectColor.Color;
        
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user))
        {
            text.text = savePrefixText + $"{user.GetTotalPuzzleCount()} puzzles.";


            // float alpha = isCurrentSaveNumber ? 1f : 0.8f;
            // background.color = new Color(background.color.r, background.color.g, background.color.b, alpha);
            //
            // if (displayRemoval)
            // {
            //     removeImage.color = new Color(removeImage.color.r, removeImage.color.g, removeImage.color.b, alpha);
            // }
        }
        else
        {
            text.text = savePrefixText + "Empty";
        }
    }

    public void LoadSave()
    {
        if (SaveManager.TrySetUser(saveNumber))
        {
            _scenePort.CallLoadPuzzleSelectScene();
        }
    }
}
