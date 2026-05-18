using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum WindowSize
{
    Small,
    Medium,
    Large
}

public class PopupContentsBehaviour : MonoBehaviour 
{
    [Header("Title")]
    public string Title;
    
    [Header("Size")]
    [SerializeField] WindowSize height = WindowSize.Medium;
    [SerializeField] WindowSize width = WindowSize.Medium;
    
    public int Height => _sizeLookup[height];
    public int Width => _sizeLookup[width];

    [Header("Content")]
    [SerializeField] private bool contentCoversFooterArea = false;

    [Header("Buttons")]
    public List<ButtonData> ButtonDatas;

    [Header("Confirm Popup Data")] 
    public bool HasExplanationText;
    public string ExplanationText;
    public bool UseCancelButton;
    public string ConfirmButtonText;
    
    private Dictionary<WindowSize, int> _sizeLookup = new Dictionary<WindowSize, int>()
    {
        { WindowSize.Small, 450 },
        { WindowSize.Medium, 900 },
        { WindowSize.Large, 1350 }
    };
    
    public void InjectConfirmContent(PopupContentsBehaviour from)
    {
        // height = from.height;
        // width = from.width;
        
        Title = from.Title;
        ExplanationText = from.ExplanationText;
        UseCancelButton = from.UseCancelButton;
        ConfirmButtonText = from.ConfirmButtonText;
    }
}

[Serializable]
public struct ButtonData
{
    public string Text;
    public Sprite Icon;
    public UnityEvent OnClick;
}
