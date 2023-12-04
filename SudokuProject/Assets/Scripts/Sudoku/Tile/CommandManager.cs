
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand();
}

public class CommandManager : MonoBehaviour
{
    private Stack<SudokuCommand> undoStack = new Stack<SudokuCommand>();
    private Stack<SudokuCommand> redoStack = new Stack<SudokuCommand>();

    
    private void OnEnable()
    {
        EventManager.OnSetupTiles += OnSetupTiles;
        
        EventManager.OnUserNumberEnter += OnUserNumberEnter;
        EventManager.OnUserRemoveEntry += OnUserRemoveEntry;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
        
        EventManager.OnUserNumberEnter -= OnUserNumberEnter;
        EventManager.OnUserRemoveEntry -= OnUserRemoveEntry;
    }

    private void OnUserNumberEnter(SudokuCommand command)
    {
        ExecuteCommand(command);
    }
    
    private void OnUserRemoveEntry(SudokuCommand command)
    {
        ExecuteCommand(command);
    }

    private void ExecuteCommand(SudokuCommand command)
    {
        EventManager.ExecuteCommand(command);
        undoStack.Push(command);
        redoStack.Clear();
    }
    
    public void Undo()
    {
        Debug.Log($"Undo called, stack count: {undoStack.Count}");
        
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            EventManager.Undo(command);
            redoStack.Push(command);
            
            Debug.Log($"Number to undo: {command.number}");
            Debug.Log($"Index: {command.tiles[0]}");
        }
    }

    public void Redo()
    {
        Debug.Log($"Redo called, stack count: {redoStack.Count}");

        if (redoStack.Count > 0) {
            SudokuCommand command = redoStack.Pop();
            EventManager.Redo(command);
            undoStack.Push(command);
            
            Debug.Log($"Number to redo: {command.number}");
            Debug.Log($"Index: {command.tiles[0]}");
        }
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
                Undo();
            else if (Input.GetKeyDown(KeyCode.Y))
                Redo();
        }
    }
}

public class SudokuCommand
{
    public List<TileBehaviour> tiles;
    public int number;
    public EnterType enterType;
    public bool entry;
    public bool colorRemoval;
    
    public SudokuCommand(List<TileBehaviour> tiles, int number, EnterType enterType, bool entry, bool colorRemoval = false)
    {
        this.tiles = tiles;
        this.number = number;
        this.enterType = enterType;
        this.entry = entry;
        this.colorRemoval = colorRemoval;
    }
}