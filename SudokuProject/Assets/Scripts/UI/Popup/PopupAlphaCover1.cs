using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupAlphaCover1 : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private PopupWindowNewBehaviour popupWindow;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        popupWindow.Close();
    }
}