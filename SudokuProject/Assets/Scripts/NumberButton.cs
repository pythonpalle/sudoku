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

    [Header("Transforms")] 
    [SerializeField] private RectTransform digitText;
    [SerializeField] private RectTransform centerText;
    [SerializeField] private RectTransform cornerText;
    [SerializeField] private RectTransform colorTransform;

    [Header("Scriptable Object")] 
    [SerializeField] private TileColors tileColors;

    private List<RectTransform> buttonStates;

    private void Start()
    {
        buttonStates = new List<RectTransform>
        {
            digitText,
            centerText,
            cornerText,
            colorTransform
        };

        SetStateNumber();
        EnterState(digitText);
    }

    private void SetStateNumber()
    {
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
        if (colorTransform.TryGetComponent(out Image image))
        {
            image.color = tileColors.Colors[number - 1];
        }
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
        
        state.gameObject.SetActive(true);
    }
}
