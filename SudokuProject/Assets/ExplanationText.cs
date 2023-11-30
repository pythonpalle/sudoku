using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private ExplanationObject explanationObject;
    [SerializeField] private string explanationText;

    private IEnumerator routine;
    private bool routineIsRunning;
    
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
        if (explanationObject.HasSpawnedObject)
        {
            explanationObject.ExplanationPopupInstance.gameObject.SetActive(true);
        }
        else
        {
            explanationObject.Canvas = FindObjectOfType<Canvas>();
            explanationObject.ExplanationPopupInstance = Instantiate(explanationObject.ExplanationPopupPrefab, explanationObject.Canvas.transform, false);
        }
        
        explanationObject.ExplanationPopupInstance.transform.position = eventData.position;
        explanationObject.ExplanationPopupInstance.TextMesh.text = explanationText;
    }

    private void ClosePopup()
    {
        if (routine != null)
            StopCoroutine(routine);
        
        routineIsRunning = false;
        
        if (explanationObject.HasSpawnedObject)
            explanationObject.ExplanationPopupInstance.gameObject.SetActive(false);
    }
}