using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool usePointerEnter = true;
    [SerializeField] private bool usePointerExit = true;
    
    public UnityEvent OnMouseHover;
    public UnityEvent OnMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandlePointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandlePointerExit();
    }

    private void HandlePointerEnter()
    {
        if (!usePointerEnter)
        {
            HandlePointerExit();
            return;
        }
        
        EventManager.UIElementHover();
        OnMouseHover?.Invoke();
    }
    
    private void HandlePointerExit()
    {
        
        if (!usePointerExit) return;

        EventManager.UIElementExit();
        OnMouseExit?.Invoke();
    }
}
