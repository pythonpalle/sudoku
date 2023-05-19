using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumberButton : MonoBehaviour
{
    [Header("Number")]
    [SerializeField] private int number;
    [SerializeField] private bool hasNumber = true;
    
    [Header("Image")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image colorImage;

    [Header("State Transforms")] 
    [SerializeField] private RectTransform digitText;
    [Space]
    
    [SerializeField] private RectTransform centerText;
    [Space]
    
    [SerializeField] private RectTransform cornerText;
    [Space]
    
    [SerializeField] private RectTransform colorTransform;
    [SerializeField] private bool useOwnColor = false;
    [SerializeField] private Color ownColor;
    [Space]

    [Header("Color")] 
    [SerializeField] private TileColors tileColors;
    [SerializeField] private ColorObject backgroundColor;
    [SerializeField] private ColorObject textColor;

    private List<RectTransform> buttonStates;

    private void Start()
    {
        SetStates();
        
        SetStateNumber();
        EnterState(digitText);
        SetColors();
        SetBackgroundColor();
    }

    private void SetStates()
    {
        buttonStates = new List<RectTransform>
        {
        };
        
        if (digitText) buttonStates.Add(digitText);
        if (cornerText) buttonStates.Add(cornerText);
        if (centerText) buttonStates.Add(centerText);
        if (colorTransform) buttonStates.Add(colorTransform);
    }

    private void SetColors()
    {
        SetBackgroundColor();
        SetTextColor();
    }

    private void SetBackgroundColor()
    {
        backgroundImage.color = backgroundColor.Color;
    }
    
    private void SetTextColor()
    {
        foreach (var state in buttonStates)
        {
            if (state.TryGetComponent(out TextMeshProUGUI text))
            {
                text.color = textColor.Color;
            }
        }
    }

    private void SetStateNumber()
    {
        if (!hasNumber) return;
        
        foreach (var buttonState in buttonStates)
        {
            if (buttonState == colorTransform)
            {
                HandleSetColor();
                continue;
            }

            if (buttonState.TryGetComponent(out TextMeshProUGUI text))
            {
                text.text = number.ToString();
            }
        }
    }

    private void HandleSetColor()
    {
        if (!colorTransform) return;

        if (useOwnColor)
        {
            colorImage.color = backgroundColor.Color;
            return;
        }
        
        colorImage.color = tileColors.Colors[number - 1];
    }

    private void OnEnable()
    {
        EventManager.OnSelectButtonClicked += OnSelectButtonClicked;
    }
    
    private void OnDisable()
    {
        EventManager.OnSelectButtonClicked -= OnSelectButtonClicked;
    }

    private void OnSelectButtonClicked(EnterType type)
    {
        switch (type)
        {
            case EnterType.DigitMark:
                EnterState(digitText);
                break;
            
            case EnterType.CenterMark:
                EnterState(centerText);
                break;
            
            case EnterType.CornerMark:
                EnterState(cornerText);
                break;
            
            case EnterType.ColorMark: 
                EnterState(colorTransform);
                break;
        }
    }
    
    private void EnterState(RectTransform state)
    {
        foreach (var buttonState in buttonStates)
        {
            buttonState.gameObject.SetActive(false);
        }

        if (state)
        {
            state.gameObject.SetActive(true);
        }
        else
        {
            EnterState(digitText);
        }
    }
}
