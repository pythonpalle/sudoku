using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/CommandPort")]
public class CommandPort : ScriptableObject
{
    public UnityAction<SudokuCommand> OnCommandExecute;

    public void ExecuteCommand(SudokuCommand command)
    {
        Debug.Log("Executing command!");
        OnCommandExecute?.Invoke(command);
    }
}