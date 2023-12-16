using Saving;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSelect
{
    public class PuzzleSelectBox : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort selectPort;
        [SerializeField] private TextMeshProUGUI nameText;
        private PuzzleDataHolder puzzle;

        [SerializeField] private List<SelectTile> tiles;

        public void SetData(PuzzleDataHolder puzzleData, bool removeUnused = false) 
        {
            puzzle = puzzleData;
            UpdateContents(removeUnused);
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

        void UpdateContents(bool removeUnusedColors = false) 
        {
            UpdateName();
            
            var numbers = puzzle.numbers;
            var permanents = puzzle.permanent;
            bool[] contradicted = puzzle.contradicted;

            for (int i = 0; i < 81; i++)
            {
                var tile = tiles[i];
                var colorMarks = puzzle.colorMarks[i];
                
                // apply all tile colors
                if (removeUnusedColors)
                {
                    tile.RemoveUnusedColors(colorMarks.Count);
                }
                tile.SetColorMarks(colorMarks, contradicted[i]);
                
                int number = numbers[i];

                // has no digit, apply corner marks and center marks
                if (number == 0)
                {
                    tile.SetCenters(puzzle.centerMarks[i]);
                    tile.SetCorners(puzzle.cornerMarks[i]);
                    tile.SetTextColor(false);
                    continue;
                }
                
                // otherwise, set digit
                tile.SetDigit(number, permanents[i]);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 81; i++)
            {
                tiles[i].Reset();
            }
        }
    }
} 

