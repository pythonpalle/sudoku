using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MenuOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    /// <summary>
    /// TODO:
    ///
    /// OnHover/OnPreview (för creation, när hovrar över auto) och OnSelect (när trycker på auto)
    ///
    /// </summary>
    
    public MenuOptionHolder parent;
    
    public TextMeshProUGUI DescriptionText;
    
    public bool selectOnHover;
    public bool selectOnClick;
    
    public bool startSelected; 
    public UnityEvent OnSelect;
    public UnityEvent OnDeselect;

    private void OnEnable()
    {
        if (startSelected)
            RequestSelect();
    }

    private void RequestSelect()
    {
        parent.RequestSelect(this);
    }
    private void RequestDeselect()
    {
        parent.RequestDeselect(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectOnHover) RequestSelect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectOnHover) RequestDeselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectOnClick) RequestSelect();
    }
    
    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }
}