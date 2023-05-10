using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnterType
{
    NormalNumber,
    CellPencilMark,
    BoxPencilMark,
    ColorMark
}

public class GridEnterManager : MonoBehaviour
{
    public static GridEnterManager Instance { get; private set; }
    
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

    private EnterType enterType = EnterType.NormalNumber;

    private bool removeButtonIsPressed => Input.GetKeyDown(KeyCode.Delete)
                                          || Input.GetKeyDown(KeyCode.Backspace)
                                          || Input.GetKeyDown(KeyCode.Alpha0)
                                          || Input.GetKeyDown(KeyCode.Keypad0);

    private void Awake()
    {
        MakeSingleton();
    }
    
    private void MakeSingleton()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        HandleNumberEnter();
        HandleNumberRemove();
    }

    private void HandleNumberEnter()
    {
        if (!SelectionManager.Instance.HasSelectedTiles)
            return;
        
        for (int number = 1; number <= 9; number++)
        {
            if (Input.GetKeyDown(NumberKeys[number]) || Input.GetKeyDown(NumberKeypadKeys[number]))
            {
                EventManager.EnterNumber(SelectionManager.Instance.SelectedTiles, enterType, number);
            }
        }
    }
    
    private void HandleNumberRemove()
    {
        if (!SelectionManager.Instance.HasSelectedTiles)
            return;

        
        if (removeButtonIsPressed)
        {
            EventManager.RemoveEntry(SelectionManager.Instance.SelectedTiles, enterType);
        }
    }
}
