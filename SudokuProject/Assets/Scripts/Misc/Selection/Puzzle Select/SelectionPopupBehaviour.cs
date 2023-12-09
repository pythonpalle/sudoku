using System;
using System.Collections;
using System.Collections.Generic;
using Saving;
using TMPro;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectionPopupBehaviour : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectPort puzzleSelectPort;
        [SerializeField] private PopupWindow _popupWindow;

        [SerializeField] private TextMeshProUGUI placeHolderText;

        private PuzzleDataHolder currentPuzzle;

        private void OnEnable()
        {
            puzzleSelectPort.OnSelectPuzzleBox += OnSelectPuzzleBox;

            SaveManager.OnPuzzleDeleted += OnPuzzleDeleted;
        }
        
        private void OnDisable()
        {
            puzzleSelectPort.OnSelectPuzzleBox -= OnSelectPuzzleBox;
            
            SaveManager.OnPuzzleDeleted -= OnPuzzleDeleted;
        }

        private void OnSelectPuzzleBox()
        {
            currentPuzzle = puzzleSelectPort.selectedPuzzle;
            _popupWindow.PopUp();
            placeHolderText.text = currentPuzzle.name;
        }

        private void OnPuzzleDeleted(PuzzleDataHolder _)
        {
            _popupWindow.Close();
        }

        public void OnPlayButtonPressed()
        {
            Debug.Log($"Update puzzle name... but then:");
            Debug.Log($"Play puzzle {currentPuzzle.name}!");

            puzzleSelectPort.SelectAndLoad(currentPuzzle);
        }
        
        public void OnDeleteButtonPressed()
        {
            EventManager.DisplayConfirmPopup(DeletePuzzle);
        }

        void DeletePuzzle()
        {
            SaveManager.TryDeletePuzzle(currentPuzzle);
        } 
        
        public void OnRestartButtonPressed()
        {
            Debug.LogWarning($"Warn for restarting puzzle {currentPuzzle.name}!");
        }
    }

}