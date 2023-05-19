using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupAlphaCover : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private PopupWindow popupWindow;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Close popup!");
        popupWindow.Close();
    }
}
