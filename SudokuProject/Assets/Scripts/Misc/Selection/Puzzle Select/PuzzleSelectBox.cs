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
        [SerializeField] private ColorObject permanentTileTextColor;
        [SerializeField] private ColorObject tileEnterColor;
        
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

        public void UpdateName()
        {
            nameText.text = puzzle.name;
        }

        void UpdateContents() 
        {
            UpdateName();
            
            var numbers = puzzle.numbers;
            var permanents = puzzle.permanent;

            for (int i = 0; i < 81; i++)
            {
                var tile = tiles[i];
                
                // apply all tile colors
                tile.SetColors(puzzle.colorMarks[i]);
                
                int number = numbers[i];
                
                // has no digit, apply corner marks and center marks
                if (number == 0)
                {
                    tile.SetTextColor(tileEnterColor.Color);
                    
                    tile.SetCenters(puzzle.centerMarks[i]);
                    tile.SetCorners(puzzle.cornerMarks[i]);
                    continue;
                }
                
                // otherwise, set digit
                Color digitColor = permanents[i] ? permanentTileTextColor.Color : tileEnterColor.Color;
                tile.SetDigit(number, digitColor);
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

