using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridEnterButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private EnterType enterType;
    
    private Color selectColor = Color.blue;
    private Color deSelectColor = Color.white;

    [SerializeField] private ColorObject selectColorObject;
    [SerializeField] private ColorObject deselectColorObject;

    private void Start()
    {
        SetColors();
        OnSelectButtonClicked(EnterType.DigitMark);
    }

    private void SetColors()
    {
        selectColor = selectColorObject.Color;
        deSelectColor = deselectColorObject.Color;
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

    private void Select()
    {
        button.image.color = selectColor;
        textMesh.color = deSelectColor;
    }
    
    private void Deselect()
    {
        button.image.color = deSelectColor;
        textMesh.color = selectColor;
    }
}
