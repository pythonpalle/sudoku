using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;

public class SaveFilesSelectSceneButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        int saveFile = SaveManager.GetSaveFile() + 1;
        text.text = $"Save {saveFile}";
    }
}
