using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private ExplanationObject explanationObject;
    [SerializeField] private string explanationText; 
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopCoroutine($"DisplayPopupAfterSeconds");
        StartCoroutine(DisplayPopupAfterSeconds(eventData, 1));
    }
    
    private IEnumerator DisplayPopupAfterSeconds(PointerEventData eventData, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        DisplayPopup(eventData);
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

    

    public void OnPointerExit(PointerEventData eventData)
    {
        ClosePopup();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        StopCoroutine($"DisplayPopupAfterSeconds");
        
        if (explanationObject.HasSpawnedObject)
            explanationObject.ExplanationPopupInstance.gameObject.SetActive(false);
    }

    
}