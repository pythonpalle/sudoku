
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand();
}

public class CommandManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnSetupTiles += OnSetupTiles;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
    }

    private void OnSetupTiles()
    {
        EventManager.OnNewCommand?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
                CallUndo();
            else if (Input.GetKeyDown(KeyCode.Y))
                CallRedo();
        }
    }

    // puplic för att ska kunna kallas på från undo/redo-knapparna
    public void CallUndo()
    {
        EventManager.Undo();
    }

    public void CallRedo()
    {
        EventManager.Redo();
    }
}

public class SudokuCommand
{
    public List<TileBehaviour> tiles;
    public int number;
    public EnterType enterType;
    public bool entry;
    
    public SudokuCommand(List<TileBehaviour> tiles, int number, EnterType enterType, bool entry)
    {
        this.tiles = tiles;
        this.number = number;
        this.enterType = enterType;
        this.entry = entry;
    }
}