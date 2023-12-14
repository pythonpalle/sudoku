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

public static class InputManager
{
    static KeyCode[] NumberKeys = {
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
    
    static KeyCode[] NumberKeypadKeys = {
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
    
    public static bool RemoveButtonIsPressed => Input.GetKeyDown(KeyCode.Delete)
                                          || Input.GetKeyDown(KeyCode.Backspace)
                                          || Input.GetKeyDown(KeyCode.Alpha0)
                                          || Input.GetKeyDown(KeyCode.Keypad0);

    public static bool NumberKeyDown(int number)
    {
        return Input.GetKeyDown(NumberKeys[number]) || Input.GetKeyDown(NumberKeypadKeys[number]);
    }
}

public class GridEnterManager : MonoBehaviour
{
    [SerializeField] private SelectionObject selectionObject;
    [SerializeField] private EnterType enterType = EnterType.DigitMark;

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
            if (InputManager.NumberKeyDown(number))
            {
                TryEnterNumber(number);
            }
        }
    }
    
    private void HandleNumberRemove()
    {
        if (InputManager.RemoveButtonIsPressed)
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
