using PuzzleSelect;
using Saving;
using UnityEngine;

public class RestartPopupContentsBehaviour : PopupContentsBehaviour
{
    [SerializeField] private PuzzleSelectPort puzzleSelectPort;


    public void Reset()
    {
        SaveManager.RestartPuzzle(puzzleSelectPort.selectedPuzzle);
    }
}
