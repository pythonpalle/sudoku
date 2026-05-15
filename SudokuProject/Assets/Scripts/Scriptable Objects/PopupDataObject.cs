using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PopupDataObject", menuName = "Scriptable Objects/PopupDataObject")]
public class PopupDataObject : ScriptableObject
{
    [Header("Title")]
    public string Title;
    
    [Header("Size")]
    [SerializeField] WindowSize height = WindowSize.Medium;
    [SerializeField] WindowSize width = WindowSize.Medium;
    
    public int Height => _sizeLookup[height];
    public int Width => _sizeLookup[width];

    [Header("Content")]
    public GameObject PopupContent;

    // [Header("Buttons")]
    // public List<ButtonData> ButtonDatas;

    [Header("Button")] 
    public bool UseCancelButton;
    public string ConfirmButtonText;
    
    private Dictionary<WindowSize, int> _sizeLookup = new Dictionary<WindowSize, int>()
    {
        { WindowSize.Small, 450 },
        { WindowSize.Medium, 900 },
        { WindowSize.Large, 1350 }
    };
    

}

