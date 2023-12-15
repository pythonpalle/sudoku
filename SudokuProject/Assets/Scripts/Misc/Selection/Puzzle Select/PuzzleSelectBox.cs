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
        [SerializeField] private ColorObject normalTileTextColor;
        
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

        public void UpdateContents()
        {
            UpdateName();
            
            var numbers = puzzle.numbers;
            var permanents = puzzle.permanent;

            for (int i = 0; i < 81; i++)
            {
                int number = numbers[i];
                if (number == 0)
                    continue;
                
                tiles[i].digitText.text = number.ToString();
                tiles[i].digitText.color = permanents[i] ? permanentTileTextColor.Color : normalTileTextColor.Color;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 81; i++)
            {
                tiles[i].digitText.text = "";
            }
        }
    }
} 

