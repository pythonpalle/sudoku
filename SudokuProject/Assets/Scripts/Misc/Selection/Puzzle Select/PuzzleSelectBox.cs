using Saving;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectTile : MonoBehaviour
    {
        public TextMesh digitText;
    }
    
    public class PuzzleSelectBox : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort selectPort;
        [SerializeField] private TextMeshProUGUI nameText;
        private PuzzleDataHolder puzzle;

        [SerializeField] private List<SelectTile> tiles;

        public void SetData(PuzzleDataHolder puzzleData)
        {
            puzzle = puzzleData;
            UpdateContents();
        }

        public void OnButtonPressed()
        {
            selectPort.SelectPuzzleBox(this, puzzle); 
        }

        public bool HasPuzzle(PuzzleDataHolder other)
        {
            return other == puzzle;
        }

        public void UpdateContents()
        {
            nameText.text = puzzle.name;

            for (int i = 0; i < 81; i++)
            {
                tiles[i].digitText.text = puzzle.numbers[i].ToString();
            }
        }
    }
} 

