using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

namespace PuzzleSelect
{
    public class PuzzleSelectList : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort _selectPort;
        
        [SerializeField] private Transform scrollContentParent;
        [SerializeField] private PuzzleSelectBox selectBoxPrefab;

        private void OnEnable()
        {
            _selectPort.OnUserDataLoaded += OnUserDataLoaded;
        }
        
        private void OnDisable()
        {
            _selectPort.OnUserDataLoaded -= OnUserDataLoaded;
        }

        private void OnUserDataLoaded(UserSaveData data)
        {
            SetSelectionBoxes(data);
        }

        void SetSelectionBoxes(UserSaveData data)
        {
            foreach (var puzzle in data.puzzles)
            {
                PuzzleSelectBox selectBoxInstance = Instantiate(selectBoxPrefab, scrollContentParent);
                selectBoxInstance.SetData(puzzle);
            }
        }
    }

}

