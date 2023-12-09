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
        }
        
        private void OnDisable()
        {
            puzzleSelectPort.OnSelectPuzzleBox -= OnSelectPuzzleBox;
        }

        private void OnSelectPuzzleBox(PuzzleDataHolder puzzleData)
        {
            currentPuzzle = puzzleData;
            _popupWindow.PopUp();
            placeHolderText.text = currentPuzzle.name;
        }

        public void OnPlayButtonPressed()
        {
            Debug.Log($"Update puzzle name... but then:");
            Debug.Log($"Play puzzle {currentPuzzle.name}!");
        }
        
        public void OnDeleteButtonPressed()
        {
            EventManager.DisplayConfirmPopup(DeletePuzzle);
        }

        void DeletePuzzle()
        {
            Debug.Log($"Telling EM to delete puzzle {currentPuzzle.name}...");
            SaveManager.TryDeletePuzzle(currentPuzzle);
        } 
        
        public void OnRestartButtonPressed()
        {
            Debug.LogWarning($"Warn for restarting puzzle {currentPuzzle.name}!");
        }
    }

}

