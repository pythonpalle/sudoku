using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MenuOptionHolder parent;
    
    public TextMeshProUGUI DescriptionText;
    
    public bool selectOnHover;
    public bool selectOnClick;
    
    public bool startSelected;
    public UnityEvent Select;
    public UnityEvent Deselect;

    public void SelectAndInform()
    {
        parent.Select(this);
        Select?.Invoke();
    }
    public void DeselectAndInform()
    {
        parent.Deselect(this);
        Deselect?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectOnHover) SelectAndInform();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectOnHover) DeselectAndInform();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick" + gameObject.name);
        
        if (selectOnClick) SelectAndInform();
    }
}