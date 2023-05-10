using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnMouseHover;
    public UnityEvent OnMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.UIElementHover();
        OnMouseHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.UIElementExit();
        OnMouseExit?.Invoke();
    }
}
