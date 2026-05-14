using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class PopupContentsBehaviour : MonoBehaviour
{
    public string Title;
    public List<ButtonData> ButtonDatas;
    public GameObject PopupContent;
}

[Serializable]
public struct ButtonData
{
    public string Text;
    public Button.ButtonClickedEvent OnClick;
}
