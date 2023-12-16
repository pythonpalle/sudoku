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

        [SerializeField] List<PuzzleSelectBox> selectBoxes;

        private void OnEnable()
        {
            _selectPort.OnUserDataLoaded += OnUserDataLoaded;
            SaveManager.OnPuzzleDeleted += OnPuzzleDeleted;
        }
        
        private void OnDisable()
        {
            _selectPort.OnUserDataLoaded -= OnUserDataLoaded;
            SaveManager.OnPuzzleDeleted -= OnPuzzleDeleted;
        }

        private void OnPuzzleDeleted(PuzzleDataHolder puzzle)
        {
            if (_selectPort.selectedBox.HasPuzzle(puzzle))
            {
                Debug.Log("Has same puzzle, remove the box!");
                Destroy(_selectPort.selectedBox.gameObject);
                _selectPort.selectedBox = null;
            } 
        }

        private void OnUserDataLoaded(UserSaveData data)
        {
            SetSelectionBoxes(data);
        }

        void SetSelectionBoxes(UserSaveData data)
        {
            StartCoroutine(SetupBoxes(data));
        }

        private IEnumerator SetupBoxes(UserSaveData data)
        {
            bool lessPuzzlesThanBoxes = true;
            
            for (var index = 0; index < data.puzzles.Count; index++)
            {
                var puzzle = data.puzzles[index];
                PuzzleSelectBox selectBox;
                if (index < selectBoxes.Count)
                {
                    selectBox = selectBoxes[index];
                    selectBoxes[index].gameObject.SetActive(true);
                }
                else
                {
                    selectBox = Instantiate(selectBoxPrefab, scrollContentParent);
                    selectBoxes.Add(selectBox);
                    lessPuzzlesThanBoxes = false;
                }
                
                selectBox.SetData(puzzle);
                yield return new WaitForEndOfFrame();
            }

            if (lessPuzzlesThanBoxes)
            {
                while (selectBoxes.Count > data.puzzles.Count)
                {
                    var lastBox = selectBoxes[^1];
                    selectBoxes.RemoveAt(selectBoxes.Count - 1);
                    Destroy(lastBox.gameObject);
                }
            }
            
        }
    }
}

