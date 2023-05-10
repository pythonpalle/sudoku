using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Vector2 defaultCursorOffset;
    
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Vector2 hoverCursorOffset;

    private void Awake()
    {
        SetDefaultCursor();
    }

    private void OnEnable()
    {
        EventManager.OnUIElementHover += OnUIElementHover;
        EventManager.OnUIElementExit += OnUIElementExit;
    }
    
    private void OnDisable()
    {
        EventManager.OnUIElementHover -= OnUIElementHover;
        EventManager.OnUIElementExit -= OnUIElementExit;
    }

    private void OnUIElementHover()
    {
        SetHoverCursor();
    }

    private void OnUIElementExit()
    {
        SetDefaultCursor();
    }

    private void SetHoverCursor()
    {
        Cursor.SetCursor(hoverCursor, hoverCursorOffset, CursorMode.Auto);
    }

    private void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultCursorOffset, CursorMode.Auto);
    }
}
