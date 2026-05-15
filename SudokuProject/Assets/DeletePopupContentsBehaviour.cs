using PuzzleSelect;
using Saving;
using UnityEngine;

public class DeletePopupContentsBehaviour : PopupContentsBehaviour
{
    [SerializeField] private PuzzleSelectPort puzzleSelectPort;
    
    public void Delete()
    {
        SaveManager.TryDeletePuzzle(puzzleSelectPort.selectedPuzzle);
        Close();
    }
}
