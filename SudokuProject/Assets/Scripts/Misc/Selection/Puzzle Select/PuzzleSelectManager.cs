using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Saving;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace PuzzleSelect
{
    public class PuzzleSelectManager : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort puzzleSelectPort;
        
        private void Start()
        {
            LoadAllPuzzles();
            ResetCurrentPuzzle();
        }

        void LoadAllPuzzles()
        {
            if (SaveManager.TrySetCurrentUserData(out UserSaveData data))
            {
                puzzleSelectPort.LoadUserData(data);
            }
        }
    
        private void ResetCurrentPuzzle()
        {
            SaveManager.SetCurrentPuzzleToNull();
        }
    }
}

