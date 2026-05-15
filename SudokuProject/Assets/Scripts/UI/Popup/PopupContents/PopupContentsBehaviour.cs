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
    private PopupWindowNewBehaviour _popupWindow;
    
    [Header("Title")]
    public string Title;
    
    [Header("Size")]
    [SerializeField] WindowSize height = WindowSize.Medium;
    [SerializeField] WindowSize width = WindowSize.Medium;
    
    public int Height => _sizeLookup[height];
    public int Width => _sizeLookup[width];

    [Header("Content")]
    public GameObject PopupContent;

    [Header("Buttons")]
    public List<ButtonData> ButtonDatas;
    
    private Dictionary<WindowSize, int> _sizeLookup = new Dictionary<WindowSize, int>()
    {
        { WindowSize.Small, 450 },
        { WindowSize.Medium, 900 },
        { WindowSize.Large, 1350 }
    };
    

    public void SetPopupWindow(PopupWindowNewBehaviour popupWindow)
    {
        this._popupWindow = popupWindow;
    }
    
    public void Close()
    {
        _popupWindow.Close();
    }
}

[Serializable]
public struct ButtonData
{
    public string Text;
    public Button.ButtonClickedEvent OnClick;
}
