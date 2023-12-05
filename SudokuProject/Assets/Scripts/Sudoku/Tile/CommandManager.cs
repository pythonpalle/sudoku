
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand();
}

public class CommandManager : MonoBehaviour
{
    private Stack<SudokuEntry> undoStack = new Stack<SudokuEntry>();
    private Stack<SudokuEntry> redoStack = new Stack<SudokuEntry>();

    
    private void OnEnable()
    {
        EventManager.OnSetupTiles += OnSetupTiles;
        
        // EventManager.OnUserNumberEnter += OnUserNumberEnter;
        // EventManager.OnUserRemoveEntry += OnUserRemoveEntry;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
        
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

    // private void ExecuteCommand(SudokuEntry entry)
    // {
    //     EventManager.ExecuteCommand(entry);
    //     undoStack.Push(entry);
    //     redoStack.Clear();
    // }
    
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
            SudokuEntry entry = redoStack.Pop();
            EventManager.Redo(entry);
            undoStack.Push(entry);
            
            Debug.Log($"Number to redo: {entry.number}");
            Debug.Log($"Index: {entry.tiles[0]}");
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