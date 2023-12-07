using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

namespace Saving
{
    public class PuzzleSaveManagerBehaviour : MonoBehaviour
    {
        private PuzzleDataHolder _puzzleDataHolder;
        private List<IHasPuzzleData> savables = new List<IHasPuzzleData>();

        void FindAllSavables()
        {
            var gameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.TryGetComponent(out IHasPuzzleData savable))
                {
                    TryAddSavable(savable);
                }
            }
        }

        bool TryGetPuzzle()
        {
            if (SaveManager.TryGetCurrentPuzzle(out PuzzleDataHolder puzzle))
            {
                _puzzleDataHolder = puzzle;
                return true;
            }

            return false;
        }
        

        public bool TryAddSavable(IHasPuzzleData hasPuzzleData)
        {
            if (!savables.Contains(hasPuzzleData))
            {
                savables.Add(hasPuzzleData);
                return true;
            }

            return false;
        }

        public bool TryRemoveSavable(IHasPuzzleData hasPuzzleData)
        {
            if (savables.Contains(hasPuzzleData))
            {
                savables.Remove(hasPuzzleData);
                return true;
            }

            return false;
        }

        private void SaveGameData()
        {
            PopulateAllPuzzleData();
        }

        private void LoadGameData()
        {
            LoadAllPuzzleData();
        }

        private void PopulateAllPuzzleData()
        {
            foreach (var savable in savables)
            {
                savable.PopulateSaveData(_puzzleDataHolder);
            }
        }
        
        private void LoadAllPuzzleData()
        {
            foreach (var savable in savables)
            {
                savable.LoadFromSaveData(_puzzleDataHolder);
            }
        }
    }
}


