
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

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        EventManager.OnSetupTiles += OnSetupTiles;
        EventManager.OnNewCommand += OnNewCommand;
        
        SaveManager.AddPopulateDataListener(this);
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
        EventManager.OnNewCommand -= OnNewCommand;
        
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
            return;

        var command = undoStack.Pop();
        command.Undo();
        redoStack.Push(command);
    }

    public void RedoCommand()
    {
        if (redoStack.Count <= 0)
            return;

        var command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
    }

    private void OnNewCommand(SudokuEntry entry)
    {
        while (entries.Count > stateCounter)
        {
            entries.RemoveAt(entries.Count-1);
        }

        SudokuEntry newEntry = new SudokuEntry(entry);
        entries.Add(newEntry);
        stateCounter++;

        entryCount = entries.Count;
    }

    private bool TryChangeState(bool undo)
    {
        if (undo)
            stateCounter--;
        else
            stateCounter++;
        
        if (undo && stateCounter <= 0)
        {
            stateCounter = 1;
            return false;
        }
        
        if (!undo && stateCounter > entries.Count)
        {
            stateCounter --;
            return false;
        }
        
        if (undo)
            EventManager.Undo();
        else
            EventManager.Redo();

        entryCount = entries.Count;

        return true;
    }

    private void OnSetupTiles()
    {
        EventManager.CallNewCommand(null);
        Debug.Log("CM Tiles set up");
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
        TryChangeState(true);
    }

    public void CallRedo()
    {
        TryChangeState(false);
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