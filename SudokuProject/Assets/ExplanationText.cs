using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextPopupPort _popupPort;
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
        _popupPort.DisplayHoverText(explanationText, eventData.position);
        
        // if (explanationObject.HasSpawnedObject)
        // {
        //     explanationObject.textMeshContainerInstance.gameObject.SetActive(true);
        // }
        // else
        // {
        //     explanationObject.Canvas = FindObjectOfType<Canvas>();
        //     explanationObject.textMeshContainerInstance = Instantiate(explanationObject.textMeshContainerPrefab, explanationObject.Canvas.transform, false);
        // }
        //
        // explanationObject.textMeshContainerInstance.transform.position = eventData.position;
        // explanationObject.textMeshContainerInstance.TextMesh.text = explanationText;
    }

    private void ClosePopup()
    {
        if (routine != null)
            StopCoroutine(routine);
        
        routineIsRunning = false;
        
        _popupPort.CancelHoverText();
        
        // if (explanationObject.HasSpawnedObject)
        //     explanationObject.textMeshContainerInstance.gameObject.SetActive(false);
    }
}