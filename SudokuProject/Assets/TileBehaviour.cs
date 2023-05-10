using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public int row;
    public int col;
    public int number;

    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Image border;
    [SerializeField] private Image whitePart;

    private string tileString => $"({row},{col})";

    public bool isSelected = false;

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
        
        EventManager.SetTileIndex(row, col, this);
    }

    public void SetNumber(int number)
    {
        this.number = number;

        numberText.text = number > 0 ? number.ToString() : string.Empty;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleTrySelect();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        HandleTrySelect();
    }

    private bool HandleTrySelect()
    {
        if (!SelectionManager.Instance.IsSelecting || isSelected)
        {
            return false;
        }

        HandleSelect();
        return true;
    }

    private void HandleSelect()
    {
        isSelected = true;
        border.color = Color.blue;
        whitePart.transform.localScale = Vector3.one * 0.9f;
        EventManager.SelectTile(this);
    }
}