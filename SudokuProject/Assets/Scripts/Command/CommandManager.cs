
using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Saving;
using UnityEngine;
using UnityEngine.Events;

public class CommandManager : MonoBehaviour, IPopulatePuzzleData, ILoadPuzzleData
{
    public static CommandManager instance { get; private set; }
    
    private Stack<SudokuCommand> redoStack = new Stack<SudokuCommand>();
    private Stack<SudokuCommand> undoStack = new Stack<SudokuCommand>();
    
    public UnityAction<List<int> , int > OnAddOneDigit;
    public UnityAction<int, int> OnAddDigitToTile;
    public UnityAction<List<int> , List<int> > OnAddMultipleDigits;
    public UnityAction<List<int>> OnRemoveDigits;
    
    public UnityAction<List<int> , int , int > OnAddMark;
    public UnityAction<List<int> , int , int > OnRemoveSingleMark;
    
    public UnityAction<List<int>, List<List<int>>, int> OnAddMarks;
    public UnityAction<int, Dictionary<EnterType, List<int>>> OnAddAllMarksToTile;
    public UnityAction<List<int>, int> OnRemoveAllMarks;

    public UnityAction<int> OnAddContradiction;

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
        SaveManager.AddLoadDataListener(this);
    }
    
    private void OnDisable()
    {
        SaveManager.RemovePopulateDataListener(this);
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
        
        dataHolder.undoCommands = StackToList(undoStack);
        dataHolder.redoCommands = StackToList(redoStack);
    }

    private List<SudokuCommand> StackToList(Stack<SudokuCommand> sudokuCommands)
    {
        Stack<SudokuCommand> stackCopy = new Stack<SudokuCommand>(sudokuCommands);
        int stackCount = stackCopy.Count;
        SudokuCommand[] commandList = new SudokuCommand[stackCount];

        for (int i = 0; i < stackCount; i++)
        {
            commandList[i] = stackCopy.Pop();
        }

        return commandList.ToList();
    }

    public void LoadFromSaveData(PuzzleDataHolder dataHolder)
    {
        redoStack = new Stack<SudokuCommand>(dataHolder.redoCommands);
        undoStack = new Stack<SudokuCommand>(dataHolder.undoCommands);
    }

    public void AddDigit(List<int> effectedIndexes, int addedDigit)
    {
        OnAddOneDigit?.Invoke(effectedIndexes, addedDigit);
    }

    public void AddDigits(List<int> effectedIndexes, List<int> addedDigits)
    {
        OnAddMultipleDigits?.Invoke(effectedIndexes, addedDigits);
    }
    
    public void AddDigitToTile(int index, int number)
    {
        OnAddDigitToTile?.Invoke(index, number);
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
    
    public void AddAllMarksToTile(int index, Dictionary<EnterType, List<int>> allMarks)
    {
        OnAddAllMarksToTile?.Invoke(index, allMarks);
    }

    public void AddContradictionToTile(int index)
    {
        OnAddContradiction?.Invoke(index);
    }
}