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
        [SerializeField] private ValidNameChecker _validNameChecker;
        [SerializeField] private PuzzleSelectBox popupBox;
        [SerializeField] private DifficultyIcon difficultyIcon;
        
        private PuzzleDataHolder currentPuzzle;
        private string lastSelectedPuzzleID = "";

        public PopupData deletePopupData;
        public PopupData restartPopupData;

        private void OnEnable()
        {
            puzzleSelectPort.OnSelectPuzzleBox += OnSelectPuzzleBox;

            deletePopupData.confirmButtonData.action = DeletePuzzle;
            restartPopupData.confirmButtonData.action = RestartPuzzle;

            SaveManager.OnPuzzleDeleted += OnPuzzleDeleted;
            SaveManager.OnPuzzleReset += OnPuzzleReset;

            _popupWindow.OnClose += OnPopupWindowClose;
        }

        private void OnDisable()
        {
            puzzleSelectPort.OnSelectPuzzleBox -= OnSelectPuzzleBox;
            
            SaveManager.OnPuzzleDeleted -= OnPuzzleDeleted;
            SaveManager.OnPuzzleReset -= OnPuzzleReset;
            
            _popupWindow.OnClose -= OnPopupWindowClose;
        }

        private void OnPopupWindowClose()
        {
            UpdatePuzzleName();
        }

        private void UpdatePuzzleName()
        {
            currentPuzzle.name = _validNameChecker.GetPuzzleSaveName();
            puzzleSelectPort.selectedBox.UpdateName();
            _validNameChecker.ResetUserEntered();
        }

        private void OnPuzzleReset(PuzzleDataHolder arg0)
        {
            _popupWindow.Close();
        }

        private void OnSelectPuzzleBox()
        {
            currentPuzzle = puzzleSelectPort.selectedPuzzle;
            _validNameChecker.SetPlaceHolder(currentPuzzle.name);

            if (lastSelectedPuzzleID == "" || currentPuzzle.id != lastSelectedPuzzleID)
            {
                popupBox.Clear();
                popupBox.SetData(currentPuzzle);
                difficultyIcon.SetDifficulty(currentPuzzle.difficulty);
            }
            
            _popupWindow.PopUp();
            lastSelectedPuzzleID = currentPuzzle.id;
        }

        private void OnPuzzleDeleted(PuzzleDataHolder _)
        {
            _popupWindow.Close();
        }

        public void OnPlayButtonPressed()
        {
            UpdatePuzzleName();
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