using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridEnterButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private EnterType enterType;
    
    [SerializeField] private Color selectColor = Color.blue;
    private Color deSelectColor = Color.white;

    [SerializeField] private SelectionObject selectionObject;

    private void Start()
    {
        if (enterType == EnterType.DigitMark)
        {
            Select();
        }
    }

    private void SendOnButtonClickEvent()
    {
        EventManager.SelectButtonClicked(enterType);
    }

    private void OnEnable()
    {
        button.onClick.AddListener(SendOnButtonClickEvent);
        EventManager.OnSelectButtonClicked += OnSelectButtonClicked;
    }
    
    private void OnDisable()
    {
        button.onClick.RemoveListener(SendOnButtonClickEvent);
        EventManager.OnSelectButtonClicked -= OnSelectButtonClicked;
    }
    
    private void OnSelectButtonClicked(EnterType type)
    {
        if (type == enterType)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }
    
    private void OnCornerKeyPressed()
    {
        OnSelectButtonClicked(EnterType.CornerMark);
    }

    private void OnCornerKeyReleased()
    {
        OnSelectButtonClicked(EnterType.DigitMark);
    }

    private void OnCenterKeyPressed()
    {
        OnSelectButtonClicked(EnterType.CenterMark);
    }

    private void OnCenterKeyReleased()
    {
        OnSelectButtonClicked(EnterType.DigitMark);
    }

    private void Select()
    {
        button.image.color = selectColor;
    }
    
    private void Deselect()
    {
        button.image.color = deSelectColor;
    }
}
