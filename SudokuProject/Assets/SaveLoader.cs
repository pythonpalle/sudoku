using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    [SerializeField] private int saveNumber;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] ScenePort _scenePort;

    private void OnEnable()
    {
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user))
        {
            text.text = $"Save {saveNumber + 1} - {user.GetTotalPuzzleCount()} puzzles.";
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
