using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/CreationObject")]
public class CreationObject : MonoBehaviour
{
    // TODO: gör om till GridPort
    public UnityAction OnRequestGrid;
    public UnityAction<SudokuGrid9x9> OnSendGridCopy;
    
    public void RequestGrid()
    {
        OnRequestGrid?.Invoke();
    }
    
    public void SendGridCopy(SudokuGrid9x9 copy)
    {
        OnSendGridCopy?.Invoke(copy);
    }
}
