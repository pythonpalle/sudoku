using Saving;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace PuzzleSelect
{
    public class PuzzleSelectBox : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort selectPort;
        [SerializeField] private TextMeshProUGUI nameText;
        private PuzzleDataHolder puzzle;

        public void SetData(PuzzleDataHolder puzzleData)
        {
            puzzle = puzzleData;
            nameText.text = puzzle.name; 
        }

        public void OnButtonPressed()
        {
            selectPort.SelectPuzzleBox(puzzle); 
        }
    }
} 

