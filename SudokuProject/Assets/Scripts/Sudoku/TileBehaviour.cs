﻿using System;
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
    
    [SerializeField] private SelectionObject selectionObject;
    
    private Vector3 whitePartSelectScale = Vector3.one * 0.9f;
    private Vector3 whitePartStartScale;

    private string tileString => $"({row},{col})";

    public bool isSelected = false;
    public bool Permanent = false;
    public bool Contradicted = false;

    private float timeOfLastClick;
    private const float maxTimeForDoubleClick = 0.2f;

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

    public bool TryUpdateNumber(int number)
    {
        if (Permanent) return false;
        
        SetNumber(number);
        numberText.color = Color.blue;
        return true;
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
}