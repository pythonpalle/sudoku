using System;
using System.Collections;
using System.Collections.Generic;
using Command;
using UnityEngine;

public class UndoButton : MonoBehaviour
{
    [Tooltip("Determines if the button is Redo or Undo")]
    public bool IsRedo = false;
    
    CommandManager commandManager => CommandManager.instance;

    private void Start()
    {
        if (commandManager != null)
        {
            commandManager.OnCommandUndo += OnCommandUndo;
        } else
        {
            Debug.LogError("No CommandManager found");
        }
        
    }
    private void OnDestroy()
    {
        if (commandManager != null)
        {
            commandManager.OnCommandUndo -= OnCommandUndo;
        } else
        {
            Debug.LogError("No CommandManager found");
        }
    }

    private void OnCommandUndo(SudokuCommand arg0)
    {
        if (commandManager.CanUndo)
        {
            Debug.Log("Can undo");
        }
        else
        {
            Debug.Log("Can not undo");
        }
    }
}
