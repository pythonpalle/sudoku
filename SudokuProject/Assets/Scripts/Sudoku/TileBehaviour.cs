using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // serialize fields
    [Header("Tile components")]
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Image border;
    [SerializeField] private Image whitePart;
    
    [Header("Scriptable objects")]
    [SerializeField] private SelectionObject selectionObject;
    
    // public fields
    public int row { get; private set; }
    public int col { get; private set; }
    public int number { get; private set; }

    private bool HasDigit => number > 0;

    public bool Permanent { get; private set; } = false;
    public bool Contradicted { get; private set; } = false;

    private List<int> centerMarks { get; set; } = new List<int>();
    private List<int> CornerMarks { get; set; } = new List<int>();

    // private fields
    private bool isSelected { get; set; } = false;

    private float timeOfLastClick;
    private const float maxTimeForDoubleClick = 0.2f;
    
    private Vector3 whitePartSelectScale = Vector3.one * 0.9f;
    private Vector3 whitePartStartScale;
    
    private void Start()
    {
        whitePartStartScale = whitePart.transform.localScale;
    }

    public void SetIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
        
        EventManager.SetTileIndex(row, col, this);
    }

    public void SetStartNumber(int number)
    {
        SetNumber(number);
        
        if (number > 0)
        {
            Permanent = true;
        }
    }

    private void SetNumber(int number)
    {
        this.number = number;
        numberText.text = number > 0 ? number.ToString() : string.Empty;
        if (number == 0)
            RemoveContradiction();
    }

    private bool TryUpdateDigit(int number, bool remove)
    {
        if (Permanent) return false;

        if (remove) number = 0;
        SetNumber(number);
        numberText.color = Color.blue;
        return true;
    }
    
    public bool TryUpdateNumber(int number, EnterType enterType, bool remove)
    {
        if (Permanent) return false;

        switch (enterType)
        {
            case EnterType.DigitMark:
                return TryUpdateDigit(number, remove);
            
            // case EnterType.CenterMark:
            //     return !HasDigit && centerMarks.Contains(number);
            //    
            //
            // case EnterType.CornerMark:
            //     return !HasDigit && CornerMarks.Contains(number);
        }

        return false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleTrySelect();
        EventManager.UIElementHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool doubleClick = Time.time < timeOfLastClick + maxTimeForDoubleClick;
        if (doubleClick && number > 0)
        {
            EventManager.SelectAllTilesWithNumber(number);
        }
        else
        {
            HandleTrySelect();
        }

        timeOfLastClick = Time.time;
    }

    private bool HandleTrySelect()
    {
        if (!selectionObject.IsSelecting || isSelected)
        {
            return false;
        }

        HandleSelect();
        return true;
    }

    public void HandleSelect()
    {
        isSelected = true;
        border.color = Color.blue;
        whitePart.transform.localScale = whitePartSelectScale;
        EventManager.SelectTile(this);
    }

    public void Deselect()
    {
        isSelected = false;
        border.color = Color.black;
        whitePart.transform.localScale = whitePartStartScale;
    }

    public void SetContradiction()
    {
        Contradicted = true;
        whitePart.color = Color.red;
    }

    public void RemoveContradiction()
    {
        Contradicted = false;
        whitePart.color = Color.white;
    }

    public bool HasSameNumber(int number, EnterType enterType)
    {
        switch (enterType)
        {
            case EnterType.DigitMark:
                return this.number == number;
            
            case EnterType.CenterMark:
                return !HasDigit && centerMarks.Contains(number);
               
            
            case EnterType.CornerMark:
                return !HasDigit && CornerMarks.Contains(number);
        }

        return false;
    }
}