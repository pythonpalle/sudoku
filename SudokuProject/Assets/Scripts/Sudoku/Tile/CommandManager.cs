
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
    
    // TODO: Undo, Redo
    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            //EventManager.Undo(command);
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0) {
            SudokuCommand command = redoStack.Pop();
            //EventManager.Redo(command);
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