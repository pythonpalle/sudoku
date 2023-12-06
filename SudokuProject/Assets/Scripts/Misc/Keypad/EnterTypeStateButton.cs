

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterTypeStateButton : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image backgroundImage;

    [Header("State Transforms")] 
    [SerializeField] private RectTransform digitText;
    [SerializeField] private RectTransform centerText;
    [SerializeField] private RectTransform cornerText;
    [SerializeField] private RectTransform colorTransform;

    [Header("Color")] 
    [SerializeField] private TileColors tileColors;
    [SerializeField] private ColorObject backgroundColor;
    [SerializeField] private ColorObject textColor;

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

        EnterState(digitText);
        SetColors();
        SetBackgroundColor();
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