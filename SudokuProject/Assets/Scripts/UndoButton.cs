using System;
using System.Collections;
using System.Collections.Generic;
using Command;
using UnityEngine;

public class UndoButton : MonoBehaviour
{
    [Tooltip("Determines if the button is Redo or Undo")]
    public bool IsRedo = false;

    private void OnEnable()
    {
        CommandManager.instance.OnCommandUndo += OnCommandUndo;
    }
    private void OnDisable()
    {
        CommandManager.instance.OnCommandUndo -= OnCommandUndo;
    }

    private void OnCommandUndo(SudokuCommand arg0)
    {
        if (CommandManager.instance.CanUndo)
        {
            Debug.Log("Can undo");
        }
        else
        {
            Debug.Log("Can not undo");
        }
    }
}
