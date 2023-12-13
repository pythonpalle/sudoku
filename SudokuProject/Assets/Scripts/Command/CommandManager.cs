
using System;
using System.Collections.Generic;
using Command;
using Saving;
using UnityEngine;
using UnityEngine.Events;

public interface IHasCommand
{
    public abstract void OnNewCommand(SudokuEntry entry);
}

public class CommandManager : MonoBehaviour, IPopulatePuzzleData
{
    public static CommandManager instance { get; private set; }
    
    [SerializeField] private GridPort _gridPort;
    
    private List<SudokuEntry> entries = new List<SudokuEntry>();

    private Stack<SudokuCommand> redoStack = new Stack<SudokuCommand>();
    private Stack<SudokuCommand> undoStack = new Stack<SudokuCommand>();

    [SerializeField] private int stateCounter;
    [SerializeField] private int entryCount;

    public UnityAction<List<int> , int > OnAddOneDigit;
    public UnityAction<List<int> , List<int> > OnAddMultipleDigits;
    public UnityAction<List<int>> OnRemoveDigits;
    
    public UnityAction<List<int> , int , int > OnAddMark;
    public UnityAction<List<int> , int , int > OnRemoveSingleMark;
    
    public UnityAction<List<int>, List<List<int>>, int> OnAddMarks;
    public UnityAction<List<int>, int> OnRemoveAllMarks;

    public UnityAction<SudokuCommand> OnCommandRedo;
    public UnityAction<SudokuCommand> OnCommandUndo;
    public UnityAction OnUndoFail;
    

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SaveManager.AddPopulateDataListener(this);
    }
    
    private void OnDisable()
    {
        SaveManager.RemovePopulateDataListener(this);
    }
    
    public void ExecuteNewCommand(SudokuCommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void UndoCommand()
    {
        if (undoStack.Count <= 0)
        {
            OnUndoFail?.Invoke();
            return;
        }
        var command = undoStack.Pop();
        command.Undo();
        OnCommandUndo?.Invoke(command);
        redoStack.Push(command);
    }

    public void RedoCommand()
    {
        if (redoStack.Count <= 0)
            return;

        var command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
        OnCommandRedo?.Invoke(command);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
                UndoCommand();
            else if (Input.GetKeyDown(KeyCode.Y))
                RedoCommand();
        }
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder, bool newSelfCreate)
    {
        // don't save commands for a self created sudoku
        if (newSelfCreate)
            return;
        
        _gridPort.RequestGrid();
        dataHolder.commands.Clear();
        
        foreach (var entry in entries)
        {
            if (entry == null || entry.tiles == null || entry.tiles.Count == 0) continue;
            
            SerializedCommandData command = EntryToCommand(entry);
            dataHolder.commands.Add(command);
        }

        dataHolder.commandCounter = stateCounter;
    }

    private SerializedCommandData EntryToCommand(SudokuEntry entry)
    {
        List<int> tiles = new List<int>();
        
        foreach (var tile in entry.tiles)
        {
            int i = tile.row * 9 + tile.col;
            tiles.Add(i);
        }
        
        SerializedCommandData data = new SerializedCommandData(tiles, (int)entry.enterType, entry.number, entry.removal, entry.colorRemoval);
        return data;
    }
    
    public void AddDigit(List<int> effectedIndexes, int addedDigit)
    {
        OnAddOneDigit?.Invoke(effectedIndexes, addedDigit);
    }

    public void AddDigits(List<int> effectedIndexes, List<int> addedDigits)
    {
        OnAddMultipleDigits?.Invoke(effectedIndexes, addedDigits);
    }

    public void RemoveDigits(List<int> effectedIndexes)
    {
        OnRemoveDigits?.Invoke(effectedIndexes);
    }

    public void AddMark(List<int> effectedIndexes, int number, int enterType)
    {
        OnAddMark?.Invoke(effectedIndexes, number, enterType);
    }

    public void RemoveSingleMark(List<int> effectedIndexes, int number, int enterType)
    {
        OnRemoveSingleMark?.Invoke(effectedIndexes, number, enterType);
    }

    public void RemoveAllMarks(List<int> effectedIndexes, int enterType)
    {
        OnRemoveAllMarks?.Invoke(effectedIndexes, enterType);
    }

    public void AddMarks(List<int> effectedIndexes, List<List<int>> previousMarks, int enterType)
    {
        OnAddMarks?.Invoke(effectedIndexes, previousMarks, enterType);
    }
}