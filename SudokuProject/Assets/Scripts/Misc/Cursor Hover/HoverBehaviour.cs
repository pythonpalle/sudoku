using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover")]
    [SerializeField] private bool usePointerEnter = true;
    [SerializeField] private bool usePointerExit = true;
    
    [Header("Explanation Text")]
    [SerializeField] private bool showExplanationText = true;
    [SerializeField] private string explanationText;
    [SerializeField] private bool closeWhenClicked = true;
    
    private IEnumerator routine;
    private bool routineIsRunning;

    private bool isPoppedUp;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        HandlePointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandlePointerExit(eventData);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        HandlePointerDown(eventData);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        HandlePointerUp(eventData);
    }
    
    private void HandlePointerEnter(PointerEventData eventData)
    {
        HoverBehaviourPointerEnter();
        ExplanationTextPointerEnter(eventData);
    }
    
    private void HandlePointerExit(PointerEventData eventData)
    {
        HoverBehaviourPointerExit();
        ExplanationTextPointerExit();
    }
    
    private void ExplanationTextPointerExit()
    {
        if (DoNotShowExplanationText()) return;
        
        ClosePopup();
    }

    private bool DoNotShowExplanationText()
    {
        return !showExplanationText || string.IsNullOrEmpty(explanationText);
    }

    private void HandlePointerDown(PointerEventData eventData)
    {
        ExplanationTextPointerDown();
    }

    private void HandlePointerUp(PointerEventData eventData)
    {
        ExplanationTextPointerUp();
    }
    
    private void ExplanationTextPointerDown()
    {
        if (DoNotShowExplanationText()) return;
        
        if (closeWhenClicked)
            ClosePopup();
    }
    
    private void ExplanationTextPointerUp()
    {
        if (DoNotShowExplanationText()) return;
        
        if (closeWhenClicked)
            ClosePopup();
    }


    private void OnDisable()
    {
        HoverBehaviourPointerExit();
    }



    private void HoverBehaviourPointerEnter()
    {
        if (!usePointerEnter)
        {
            HoverBehaviourPointerExit();
            return;
        }

        EventManager.UIElementHover();
    }
    
    private void ExplanationTextPointerEnter(PointerEventData eventData)
    {
        if (DoNotShowExplanationText()) return;
        
        if (routineIsRunning)
            ClosePopup();

        routine = DisplayPopupAfterSeconds(eventData, 1);
        StartCoroutine(routine);
    }

    private void HoverBehaviourPointerExit()
    {
        if (!usePointerExit) return;

        EventManager.UIElementExit();
    }
    
    public void SetText(string text)
    {
        explanationText = text;
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
