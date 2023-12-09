using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PopupWindow))]
public class ConfirmPopupWindow : MonoBehaviour
{
    private Action confirmAction;

    private PopupWindow _popupWindow;
    [SerializeField] private bool closeOnConfirm;

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

    private void OnDisplayConfirmPopup(Action action)
    {
        _popupWindow.PopUp();
        confirmAction = action;
    }

    public void OnConfirmButtonPressed()
    {
        if (confirmAction == null)
        {
            Debug.LogError("No confirm action to perform!");
            return;
        }
        
        confirmAction?.Invoke();

        if (closeOnConfirm)
        {
            _popupWindow.Close();
        }
    }
}
