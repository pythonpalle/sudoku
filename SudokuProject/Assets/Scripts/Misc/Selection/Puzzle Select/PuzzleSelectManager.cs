using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

namespace PuzzleSelect
{
    public class PuzzleSelectManager : MonoBehaviour
    {
        // [SerializeField] private Transform scrollContentParent;
        // [SerializeField] private PuzzleSelectBox selectBoxPrefab;
        [SerializeField] private PuzzleSelectPort puzzleSelectPort;

        private void Start()
        {
            LoadAllPuzzles();
            ResetCurrentPuzzle();
        }

        void LoadAllPuzzles()
        {
            if (SaveManager.TryGetCurrentUserData(out UserSaveData data))
            {
                puzzleSelectPort.LoadUserData(data);
            
                // foreach (var puzzle in data.puzzles)
                // {
                //     PuzzleSelectBox selectBoxInstance = Instantiate(selectBoxPrefab, scrollContentParent);
                //     selectBoxInstance.SetData(puzzle);
                // }
            }
        }
    
        private void ResetCurrentPuzzle()
        {
            SaveManager.ResetCurrentPuzzle();
        }
    }
}

