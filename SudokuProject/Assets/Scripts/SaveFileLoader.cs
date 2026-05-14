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
    
    [Header("Removal")]
    [SerializeField] Button removeButton;
    [SerializeField] Image removeImage;
    [SerializeField] bool displayRemoval;

    [Header("Popup")] 
    [SerializeField] private PopupData firstPopupData;
    [SerializeField] private PopupData secondPopupData;

    private void Awake()
    {
        firstPopupData.confirmButtonData.action = FirstDeleteConfirmAction;
        secondPopupData.confirmButtonData.action = SecondDeleteConfirmAction;

        TestSave();
    }
    
    private static readonly string fileExtenstion = ".dat";
    
    private static string GetFullFilePathName(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName) + fileExtenstion;
    }

    private void TestSave()
    {
        byte[] bytes = new byte[] { 0, 12, 43, 225, 255, 1 };

        string path = GetFullFilePathName("test");

        // Write binary data using FileStream
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            fileStream.Write(bytes, 0, bytes.Length);
            Debug.Log("Written!");
        }
        
        // UserSaveData saveData = new UserSaveData(new UserIdentifier("Hej", "123"));
        // var bytes = saveData.ToBinary();
        //     
        // FileManager.WriteAllBytes("test.txt", bytes, false);
    }

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
        
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user))
        {
            text.text = savePrefixText + $"{user.GetTotalPuzzleCount()} puzzles";
        }
        else
        {
            text.text = savePrefixText + "Empty";
            removeButton.gameObject.SetActive(false);
        }
    }

    public void OnDeleteButtonPressed()
    {
        EventManager.DisplayConfirmPopup(firstPopupData);
    }

    public void LoadSave()
    {
        if (SaveManager.TrySetUser(saveNumber))
        {
            _scenePort.CallLoadPuzzleSelectScene();
        }
    }
}
