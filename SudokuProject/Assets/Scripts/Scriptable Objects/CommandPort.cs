using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/CommandPort")]
public class CommandPort : ScriptableObject
{
    public UnityAction<SudokuEntry> OnCommandExecute;

    public void ExecuteCommand(SudokuEntry entry)
    {
        Debug.Log("Executing command!");
        OnCommandExecute?.Invoke(entry);
    }
}