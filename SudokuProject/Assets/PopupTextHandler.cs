using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupTextHandler : MonoBehaviour
{
    [Header("Popup texts")] 
    [SerializeField] private TextMeshContainer hoverExplanation;
    [SerializeField] private TextMeshContainer floatingPopup;

    [Header("Ports")] 
    [SerializeField] private TextPopupPort textPopupPort;

    private void OnEnable()
    {
        textPopupPort.OnDisplayHoverText += OnDisplayHoverText;
        textPopupPort.OnCancelHoverText += OnCancelHoverText;
    }
    
    private void OnDisable()
    {
        textPopupPort.OnDisplayHoverText -= OnDisplayHoverText;
        textPopupPort.OnCancelHoverText -= OnCancelHoverText;
    }

    private void OnDisplayHoverText(string text, Vector2 position)
    {
        Debug.Log("On display hover");
        
        hoverExplanation.gameObject.SetActive(true);
        hoverExplanation.transform.position = position;
        hoverExplanation.TextMesh.text = text;
    }
    
    private void OnCancelHoverText()
    {
        hoverExplanation.gameObject.SetActive(false);
    }
}
