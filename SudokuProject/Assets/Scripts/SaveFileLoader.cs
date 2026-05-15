using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField] UserSavePort _userSavePort;
    
    [Header("Removal")]
    [SerializeField] Button removeButton;
    [SerializeField] Image removeImage;
    [SerializeField] bool displayRemoval;

    [Header("Popup")] 
    [SerializeField] private PopupData firstPopupData;
    [SerializeField] private PopupData secondPopupData;
    
    [SerializeField] private PopupActivatorBehaviour deletePopupActivator;

    private void Awake()
    {
        firstPopupData.confirmButtonData.action = FirstDeleteConfirmAction;
        secondPopupData.confirmButtonData.action = SecondDeleteConfirmAction;

        //TestSave();
    }
    
    private static readonly string fileExtenstion = ".dat";

    private void FirstDeleteConfirmAction()
    {
        EventManager.DisplayConfirmPopup(secondPopupData);
    }

    private void SecondDeleteConfirmAction()
    {
        Debug.Log($"Trying to delete save {saveNumber}...");
        if (SaveManager.TryDeleteUserSave(saveNumber))
        {
            Debug.Log($"Successful delete!");
            transform.parent.gameObject.SetActive(false); 
            transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        removeButton.gameObject.SetActive(displayRemoval);
        
        string savePrefixText = $"Save {saveNumber + 1} - ";

        bool isCurrentSaveNumber = SaveManager.currentSaveNumber == saveNumber;

        Color applyColor = isCurrentSaveNumber ? selectColor.Color : deselectColor.Color;
        
        background.color = applyColor;
        removeImage.color = deselectColor.Color;//applyColor; 

        bool displayRemoveButton = false;
        
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user))
        {
            int puzzleCount = user.GetTotalPuzzleCount();
            string puzzlesText = "Empty";

            if (puzzleCount > 0)
            {
                puzzlesText = $"{puzzleCount} " + (puzzleCount == 1 ? "Puzzle" : "Puzzles");
                displayRemoveButton = true;
            }
            
            text.text = $"{savePrefixText} {puzzlesText}";
        }
        else
        {
            text.text = savePrefixText + "Empty";
        }
        
        removeButton.gameObject.SetActive(displayRemoveButton);
    }

    public void OnDeleteButtonPressed()
    {
        _userSavePort.SelectedIndexForDelete = saveNumber;
        deletePopupActivator.ActivatePopup();
    }

    public void LoadSave()
    {
        if (SaveManager.TrySetUser(saveNumber))
        {
            _scenePort.CallLoadPuzzleSelectScene();
        }
    }
}
