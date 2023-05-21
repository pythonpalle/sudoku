
using System;
using UnityEngine;

public interface IHasCommand
{
    public abstract void OnNewCommand();
}

public class CommandManager : MonoBehaviour
{
    private int commandCounter;
    
    private void OnEnable()
    {
        EventManager.OnNewCommand += OnNewCommand;
        EventManager.OnSetupTiles += OnSetupTiles;
    }
    
    private void OnDisable()
    {
        EventManager.OnNewCommand -= OnNewCommand;
        EventManager.OnSetupTiles -= OnSetupTiles;
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

    public void CallUndo()
    {
        EventManager.Undo();
    }

    public void CallRedo()
    {
        EventManager.Redo();
    }


    private void OnNewCommand()
    {
        commandCounter++;
    }
}