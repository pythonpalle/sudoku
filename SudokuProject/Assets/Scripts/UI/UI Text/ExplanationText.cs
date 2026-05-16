using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour //, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string explanationText;
    [SerializeField] private bool closeWhenClicked = true;

    private IEnumerator routine; 
    private bool routineIsRunning;

    private bool isPoppedUp;
    
    // public void SetText(string text)
    // {
    //     explanationText = text;
    // }
    //
    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     ExplanationTextPointerEnter(eventData);
    // }
    //
    // private void ExplanationTextPointerEnter(PointerEventData eventData)
    // {
    //     if (routineIsRunning)
    //         ClosePopup();
    //
    //     routine = DisplayPopupAfterSeconds(eventData, 1);
    //     StartCoroutine(routine);
    // }
    //
    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     ExplanationTextPointerExit();
    // }
    //
    // private void ExplanationTextPointerExit()
    // {
    //     ClosePopup();
    // }
    //
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     ExplanationTextPointerDown();
    // }
    //
    // private void ExplanationTextPointerDown()
    // {
    //     if (closeWhenClicked)
    //         ClosePopup();
    // }
    //
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     ExplanationTextPointerUp();
    // }
    //
    // private void ExplanationTextPointerUp()
    // {
    //     if (closeWhenClicked)
    //         ClosePopup();
    // }
    //
    // private IEnumerator DisplayPopupAfterSeconds(PointerEventData eventData, float seconds)
    // {
    //     routineIsRunning = true;
    //     
    //     yield return new WaitForSeconds(seconds);
    //     DisplayPopup(eventData);
    //
    //     routineIsRunning = false;
    // }
    //
    // private void DisplayPopup(PointerEventData eventData)
    // {
    //     EventManager.DisplayHoverText(explanationText, eventData.position);
    //     isPoppedUp = true;
    // }
    //
    // private void ClosePopup()
    // {
    //     if (routine != null && routineIsRunning)
    //         StopCoroutine(routine);
    //     
    //     if (!isPoppedUp)
    //         return;
    //     
    //     routineIsRunning = false;
    //     EventManager.CancelHoverText();
    //     isPoppedUp = false;
    // }
}