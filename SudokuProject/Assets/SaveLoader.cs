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
        if (SaveManager.TryGetUser(saveNumber, out UserSaveData user, true))
        {
            text.text = $"Save {saveNumber + 1} - {user.GetTotalPuzzleCount()} puzzles.";
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
