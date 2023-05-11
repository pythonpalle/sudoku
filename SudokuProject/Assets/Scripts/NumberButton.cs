using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NumberButton : MonoBehaviour
{
    [Header("Number")]
    [SerializeField] private int number;

    [Header("Transforms")] 
    [SerializeField] private RectTransform digitText;
    [SerializeField] private RectTransform centerText;
    [SerializeField] private RectTransform cornerText;

    private List<RectTransform> buttonStates;

    private void Start()
    {
        buttonStates = new List<RectTransform>
        {
            digitText,
            centerText,
            cornerText
        };

        SetStateNumber();
        EnterState(digitText);
    }

    private void SetStateNumber()
    {
        foreach (var buttonState in buttonStates)
        {
            if (buttonState.TryGetComponent(out TextMeshProUGUI text))
            {
                text.text = number.ToString();
            }
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
