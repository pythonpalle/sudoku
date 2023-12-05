
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

        EventManager.OnAddCommand += OnAddCommand;

        // EventManager.OnUserNumberEnter += OnUserNumberEnter;
        // EventManager.OnUserRemoveEntry += OnUserRemoveEntry;
    }

    private void OnAddCommand(SudokuCommand command)
    {
        ExecuteCommand(command);
    }

    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
        
        EventManager.OnAddCommand -= OnAddCommand;

        // EventManager.OnUserNumberEnter -= OnUserNumberEnter;
        // EventManager.OnUserRemoveEntry -= OnUserRemoveEntry;
    }

    // private void OnUserNumberEnter(SudokuEntry entry)
    // {
    //     ExecuteCommand(entry);
    // }
    //
    // private void OnUserRemoveEntry(SudokuEntry entry)
    // {
    //     ExecuteCommand(entry);
    // }

    private void ExecuteCommand(SudokuCommand command)
    {
        command.Execute();
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
            command.Undo();
            EventManager.Undo(command);
            redoStack.Push(command);
        }
    }
    
    public void Redo()
    {
        Debug.Log($"Redo called, stack count: {redoStack.Count}");
    
        if (redoStack.Count > 0) {
            SudokuCommand command = redoStack.Pop();
            command.Execute();
            EventManager.Redo(command);
            undoStack.Push(command);
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