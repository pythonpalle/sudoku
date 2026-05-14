using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct PopupData
{
    public string header;
    public string explanation;
    public ConfirmButtonData cancelButtonData;
    public ConfirmButtonData confirmButtonData;
}

[System.Serializable]
public struct ConfirmButtonData
{
    public bool exists;
    
    public Action action;
    public string text;
}

[RequireComponent(typeof(PopupWindow))]
public class ConfirmPopupWindow : MonoBehaviour
{
    private PopupData _popupData;

    private PopupWindow _popupWindow;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI explanation;

    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private TextMeshProUGUI cancelText;

    private void Start()
    {
        _popupWindow = GetComponent<PopupWindow>();
    }

    private void OnEnable()
    {
        EventManager.OnDisplayConfirmPopup += OnDisplayConfirmPopup;
    }
    
    private void OnDisable()
    {
        EventManager.OnDisplayConfirmPopup -= OnDisplayConfirmPopup;
    }

    private void OnDisplayConfirmPopup(PopupData data)
    {
        _popupWindow.PopUp();
        _popupData = data;

        header.text = _popupData.header;
        explanation.text = _popupData.explanation;

        confirmText.text = _popupData.confirmButtonData.text;
        cancelText.text = _popupData.cancelButtonData.text;

        cancelButton.gameObject.SetActive(_popupData.cancelButtonData.exists);
        confirmButton.gameObject.SetActive(_popupData.confirmButtonData.exists);
    }

    public void OnConfirmButtonPressed()
    {
        _popupWindow.Close();

        if (_popupData.confirmButtonData.action != null)
        {
            _popupData.confirmButtonData.action?.Invoke();
        }
    }

    public void CancelButtonPressed()
    {
        if (_popupData.cancelButtonData.action != null)
        {
            _popupData.cancelButtonData.action?.Invoke();
        }
        
        _popupWindow.Close();
    }
}
