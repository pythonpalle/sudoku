using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class SavePopupBehaviour : MonoBehaviour
{
    [SerializeField] private PopupWindow popupWindow;

    private void OnEnable()
    {
        SaveManager.OnRequestFirstSave += OnRequestFirstSave;
    }
    
    private void OnDisable()
    {
        SaveManager.OnRequestFirstSave -= OnRequestFirstSave;
    }

    private void OnRequestFirstSave()
    {
        Debug.Log("Popup Save Window!");
        popupWindow.PopUp();
    }
    
    public void OnYesButtonPressed()
    {
        // Get Name from inputField, send data to SaveManager,
        // let Save Manager save
    }

    public void OnNoButtonPressed()
    {
        // if pressed return to puzzle select: go to puzzle select
        
        // else: close popup
    }
    
    
}
