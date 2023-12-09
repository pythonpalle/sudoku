
using System;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand(SudokuEntry entry);
}

public class CommandManager : MonoBehaviour
{
    private List<SudokuEntry> entries = new List<SudokuEntry>();
    public List<int> counters = new List<int>();
    private int stateCounter;

    private void OnEnable()
    {
        EventManager.OnSetupTiles += OnSetupTiles;
        EventManager.OnNewCommand += OnNewCommand;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupTiles -= OnSetupTiles;
        EventManager.OnNewCommand -= OnNewCommand;
    }

    private void OnNewCommand(SudokuEntry entry)
    {
        while (entries.Count > stateCounter)
        {
            entries.RemoveAt(entries.Count-1);
            counters.Remove(counters.Count - 1);
        }
        
        entries.Add(entry);
        counters.Add(stateCounter);
        stateCounter++;
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

        return true;
    }

    private void OnSetupTiles()
    {
        EventManager.CallNewCommand(null);
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
}