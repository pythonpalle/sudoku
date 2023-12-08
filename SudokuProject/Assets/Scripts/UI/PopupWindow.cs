using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private RectTransform popupWindow;

    private bool isPopped;

    public void PopUp()
    {
        popupWindow.gameObject.SetActive(true);
        GameStateManager.OnPopup();
        isPopped = true;
    }

    public void Close()
    {
        popupWindow.gameObject.SetActive(false);
        GameStateManager.OnPopupClose();
        isPopped = false;
    }

    private void OnDestroy()
    {
        if (isPopped)
        {
            Close();
        }
    }
}
