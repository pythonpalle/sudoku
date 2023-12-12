using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private RectTransform popupWindow;

    private bool isPopped;

    public UnityAction OnPopup;
    public UnityAction OnClose;

    public void PopUp()
    {
        popupWindow.gameObject.SetActive(true);
        GameStateManager.OnPopup();
        isPopped = true;
        OnPopup?.Invoke();
    }

    public void Close()
    {
        popupWindow.gameObject.SetActive(false);
        GameStateManager.OnPopupClose();
        isPopped = false;
        OnClose?.Invoke();
    }

    private void OnDestroy()
    {
        if (isPopped)
        {
            Close();
        }
    }
}
