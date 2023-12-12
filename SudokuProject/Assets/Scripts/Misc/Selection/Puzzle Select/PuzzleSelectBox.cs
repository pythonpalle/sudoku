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
            UpdateName();
        }

        public void OnButtonPressed()
        {
            selectPort.SelectPuzzleBox(this, puzzle); 
        }

        public bool HasPuzzle(PuzzleDataHolder other)
        {
            return other == puzzle;
        }

        public void UpdateName()
        {
            nameText.text = puzzle.name; 
        }
    }
} 

