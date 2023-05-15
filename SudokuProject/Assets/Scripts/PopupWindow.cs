using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private RectTransform popupWindow;

    public void PopUp()
    {
        Debug.Log("POP UP!");
        popupWindow.gameObject.SetActive(true);
    }

    public void Close()
    {
        popupWindow.gameObject.SetActive(false);
    }
}
