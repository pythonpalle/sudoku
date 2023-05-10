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

    private EnterType enterType = EnterType.NormalNumber;

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
    }

    private void HandleNumberEnter()
    {
        if (!SelectionManager.Instance.HasSelectedTiles)
            return;
        
        for (int number = 1; number <= 9; number++)
        {
            if (Input.GetKeyDown(NumberKeys[number]))
            {
                EventManager.EnterNumber(SelectionManager.Instance.SelectedTiles, enterType, number);
            }
        }
    }
}
