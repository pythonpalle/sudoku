using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class PuzzleSelectManager : MonoBehaviour
{
    private void Awake()
    {
        LoadAllPuzzles();
        ResetCurrentPuzzle();
    }

    void LoadAllPuzzles()
    {
        if (SaveManager.TryGetCurrentUserData(out UserSaveData data))
        {
            int savedPuzzles = data.puzzles.Count;
            Debug.Log($"Puzzles saved: {savedPuzzles}");
            
            foreach (PuzzleDifficulty difficulty in Enum.GetValues(typeof(PuzzleDifficulty)))
            {
                Debug.Log($"difficulty: {difficulty}" + SaveManager.GetPuzzleCount(difficulty));
            }
        }
    }
    
    private void ResetCurrentPuzzle()
    {
        SaveManager.ResetCurrentPuzzle();
    }
}
