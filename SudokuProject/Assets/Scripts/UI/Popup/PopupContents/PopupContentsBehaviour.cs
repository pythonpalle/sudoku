using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class PopupContentsBehaviour : MonoBehaviour
{
    protected PopupWindowNewBehaviour popupWindow;
    
    public string Title;
    public List<ButtonData> ButtonDatas;
    public GameObject PopupContent;

    public void SetPopupWindow(PopupWindowNewBehaviour popupWindow)
    {
        this.popupWindow = popupWindow;
    }
    
    public void Close()
    {
        popupWindow.Close();
    }
}

[Serializable]
public struct ButtonData
{
    public string Text;
    public Button.ButtonClickedEvent OnClick;
}
