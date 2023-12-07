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
    }

    void LoadAllPuzzles()
    {
        if (SaveManager.TryGetCurrentUserData(out UserSaveData data))
        {
            int savedPuzzles = data.puzzles.Count;
            Debug.Log($"Puzzles saved: {savedPuzzles}");
        }
    }
}
