using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.UIElementHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.UIElementExit();
    }
}
