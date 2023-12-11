
using System;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand(SudokuEntry entry);
}

public class CommandManager : MonoBehaviour, IPopulatePuzzleData
{
    [SerializeField] private GridPort _gridPort;
    
    private List<SudokuEntry> entries = new List<SudokuEntry>();
    
    [SerializeField] private int stateCounter;
    [SerializeField] private int entryCount;

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
}