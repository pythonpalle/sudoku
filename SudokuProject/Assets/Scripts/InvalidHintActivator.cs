using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvalidHintActivator : MonoBehaviour
{
    [SerializeField] private PopupWindow _popupWindow;
    [SerializeField] private HintObject _hintObject;
     
    [SerializeField] private TextMeshProUGUI warningTextMesh;

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
        warningTextMesh.text = warningText;
        _popupWindow.PopUp();
    }
}
