using System;
using UnityEngine;

[RequireComponent(typeof(PopupWindow))]
public class CloseWindowOnPuzzleRemove : MonoBehaviour
{
    private PopupWindow _window;
    
    private void Start()
    {
        _window = GetComponent<PopupWindow>();
    }
}