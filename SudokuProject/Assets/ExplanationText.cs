using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string explanationText;

    private IEnumerator routine;
    private bool routineIsRunning;

    private bool isPoppedUp;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (routineIsRunning)
            ClosePopup();

        routine = DisplayPopupAfterSeconds(eventData, 1);
        StartCoroutine(routine);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        ClosePopup();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        ClosePopup();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        ClosePopup();
    }
    
    private IEnumerator DisplayPopupAfterSeconds(PointerEventData eventData, float seconds)
    {
        routineIsRunning = true;
        
        yield return new WaitForSeconds(seconds);
        DisplayPopup(eventData);

        routineIsRunning = false;
    }

    private void DisplayPopup(PointerEventData eventData)
    {
        EventManager.DisplayHoverText(explanationText, eventData.position);
        isPoppedUp = true;
    }

    private void ClosePopup()
    {
        if (routine != null && routineIsRunning)
            StopCoroutine(routine);
        
        if (!isPoppedUp)
            return;
        
        routineIsRunning = false;
        EventManager.CancelHoverText();
        isPoppedUp = false;
    }
}