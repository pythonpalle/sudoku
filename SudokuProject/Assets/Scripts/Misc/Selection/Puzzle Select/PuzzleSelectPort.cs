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

        public void LoadUserData(UserSaveData saveData)
        {
            OnUserDataLoaded?.Invoke(saveData);
        }
    }

}

