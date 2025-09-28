using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class NewPuzzleSetting : MonoBehaviour, IMenuOption, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI DescriptionText;
    public bool startSelected;
    public abstract void Select();
    public abstract void SelectStart();

    public abstract void Deselect();
    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }
}

public interface IMenuOption
{
    void Select();
    
    void Deselect();
}