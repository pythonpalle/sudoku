using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnterType
{
    DigitMark,
    CornerMark,
    CenterMark,
    ColorMark
}

public class GridEnterManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;
    
    private KeyCode[] NumberKeys = {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };
    
    private KeyCode[] NumberKeypadKeys = {
        KeyCode.Keypad0,
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9
    };

    [SerializeField] private EnterType enterType = EnterType.DigitMark;

    private bool removeButtonIsPressed => Input.GetKeyDown(KeyCode.Delete)
                                          || Input.GetKeyDown(KeyCode.Backspace)
                                          || Input.GetKeyDown(KeyCode.Alpha0)
                                          || Input.GetKeyDown(KeyCode.Keypad0);

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
        enterType = type;
    }

    private void Update()
    {
        if (!GameStateManager.gameIsActive)
            return;
        
        HandleNumberEnter();
        HandleNumberRemove();
    }

    private void HandleNumberEnter()
    {
        if (!selectionObject.HasSelectedTiles)
            return;
        
        for (int number = 1; number <= 9; number++)
        {
            if (Input.GetKeyDown(NumberKeys[number]) || Input.GetKeyDown(NumberKeypadKeys[number]))
            {
                TryEnterNumber(number);
            }
        }
    }
    
    private void HandleNumberRemove()
    {
        if (removeButtonIsPressed)
            TryRemoveNumbers();
    }

    public void TryEnterNumber(int number)
    {
        if (!selectionObject.HasSelectedTiles)
        {
            return;
        }

        SudokuEntry entry = new SudokuEntry(selectionObject.SelectedTiles, enterType, number, false, false);
        EventManager.GridEnterFromUser(entry);
    }

    public void TryRemoveNumbers(bool colorRemoval = false)
    {
        if (!selectionObject.HasSelectedTiles)
            return;
        
        SudokuEntry entry = new SudokuEntry(selectionObject.SelectedTiles, enterType, 0, true, colorRemoval);
        EventManager.GridEnterFromUser(entry);
    }
}
