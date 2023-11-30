using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ExplanationObject explanationObject;
    [SerializeField] private string explanationText; 
    
    public void OnPointerEnter(PointerEventData eventData)
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
        explanationObject.ExplanationPopupInstance.gameObject.SetActive(false);
    }
}