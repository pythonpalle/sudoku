using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvalidHintActivator : MonoBehaviour
{
    [SerializeField] private HintObject _hintObject;
    [SerializeField] private PopupData _popupData;

    private void OnEnable()
    {
        _hintObject.OnDisplayWarning += OnDisplayWarning;
    }
    
    private void OnDisable()
    {
        _hintObject.OnDisplayWarning -= OnDisplayWarning;
    }

    private void OnDisplayWarning(string warningText)
    {
        _popupData.explanation = warningText;
        
        EventManager.DisplayConfirmPopup(_popupData);
    }
}
