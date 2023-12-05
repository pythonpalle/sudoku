
using System;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand(SudokuEntry entry);
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
        EventManager.Undo();
    }

    public void CallRedo()
    {
        EventManager.Redo();
    }
}