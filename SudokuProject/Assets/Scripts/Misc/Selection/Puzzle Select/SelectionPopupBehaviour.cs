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

        public PopupData deletePopupData;
        public PopupData restartPopupData;

        private void OnEnable()
        {
            puzzleSelectPort.OnSelectPuzzleBox += OnSelectPuzzleBox;

            deletePopupData.confirmButtonData.action = DeletePuzzle;
            restartPopupData.confirmButtonData.action = RestartPuzzle;

            SaveManager.OnPuzzleDeleted += OnPuzzleDeleted;
            SaveManager.OnPuzzleReset += OnPuzzleReset;
        }

        private void OnDisable()
        {
            puzzleSelectPort.OnSelectPuzzleBox -= OnSelectPuzzleBox;
            
            SaveManager.OnPuzzleDeleted -= OnPuzzleDeleted;
            SaveManager.OnPuzzleReset -= OnPuzzleReset;
        }

        private void OnPuzzleReset(PuzzleDataHolder arg0)
        {
            _popupWindow.Close();
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
            EventManager.DisplayConfirmPopup(deletePopupData);
        }
        
        public void OnRestartButtonPressed()
        {
            EventManager.DisplayConfirmPopup(restartPopupData);
        }

        void DeletePuzzle()
        {
            SaveManager.TryDeletePuzzle(currentPuzzle);
        } 
        
        private void RestartPuzzle()
        {
            SaveManager.RestartPuzzle(currentPuzzle);
        }
    }
}