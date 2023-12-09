using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSelect
{
    [CreateAssetMenu(menuName = "Sudoku/Ports/PuzzleSelect")]
    public class PuzzleSelectPort : ScriptableObject
    {
        public UnityAction<UserSaveData> OnUserDataLoaded;
        public UnityAction<PuzzleDataHolder> OnSelectPuzzleBox;

        public void LoadUserData(UserSaveData saveData)
        {
            OnUserDataLoaded?.Invoke(saveData);
        }

        public void SelectPuzzleBox(PuzzleDataHolder puzzle)
        {
            OnSelectPuzzleBox?.Invoke(puzzle);
        }
    }

}

