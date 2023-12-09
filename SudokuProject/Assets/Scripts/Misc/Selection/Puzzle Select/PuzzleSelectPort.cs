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
        public UnityAction<PuzzleDataHolder> OnSelectAndLoad;
        
        public UnityAction OnSelectPuzzleBox;

        public PuzzleSelectBox selectedBox;
        public PuzzleDataHolder selectedPuzzle;

        public void LoadUserData(UserSaveData saveData)
        {
            OnUserDataLoaded?.Invoke(saveData);
        }

        public void SelectPuzzleBox(PuzzleSelectBox box, PuzzleDataHolder data)
        {
            selectedBox = box;
            selectedPuzzle = data;
            
            OnSelectPuzzleBox?.Invoke();
        }

        public void SelectAndLoad(PuzzleDataHolder currentPuzzle)
        {
            OnSelectAndLoad?.Invoke(currentPuzzle);
        }
    }

}

