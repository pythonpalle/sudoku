using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class PuzzleSelectManager : MonoBehaviour
{
    [SerializeField] private Transform scrollContentParent;
    [SerializeField] private PuzzleSelectBox selectBoxPrefab;

    private void Awake()
    {
        LoadAllPuzzles();
        ResetCurrentPuzzle();
    }

    void LoadAllPuzzles()
    {
        if (SaveManager.TryGetCurrentUserData(out UserSaveData data))
        {
            foreach (var puzzle in data.puzzles)
            {
                PuzzleSelectBox selectBoxInstance = Instantiate(selectBoxPrefab, scrollContentParent);
                selectBoxInstance.SetData(puzzle);
            }
        }
    }
    
    private void ResetCurrentPuzzle()
    {
        SaveManager.ResetCurrentPuzzle();
    }
}