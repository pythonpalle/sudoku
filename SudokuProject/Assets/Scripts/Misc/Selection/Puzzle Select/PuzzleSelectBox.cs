using Saving;
using TMPro;
using UnityEngine;

public class PuzzleSelectBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    private PuzzleDataHolder puzzle;

    public void SetData(PuzzleDataHolder puzzleData)
    {
        puzzle = puzzleData;
        nameText.text = puzzle.name; 
    }
}