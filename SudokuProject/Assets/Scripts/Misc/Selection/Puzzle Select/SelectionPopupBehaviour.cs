using Saving;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectionPopupBehaviour : MonoBehaviour
    {
        [Header("Assignables")]
        [SerializeField] private ValidNameChecker _validNameChecker;
        [SerializeField] private PuzzleSelectBox popupBox;
        [SerializeField] private ProgressionIcon progressionIcon;
        [SerializeField] private DifficultyIcon difficultyIcon;
        
        [Header("Ports")]
        [SerializeField] private PuzzleSelectPort puzzleSelectPort;
        
        [Header("Popup Data")]
        [SerializeField] private PopupContentsBehaviour restartPopupDataObject;
        [SerializeField] private PopupContentsBehaviour deletePopupDataObject;
        //[SerializeField] private PopupDataObject puzzleSelectPopupData;
        
        private PuzzleDataHolder currentPuzzle;
        private string lastSelectedPuzzleID = "";
        
        private void OnEnable()
        {
            OnSelectPuzzleBox();
        }
        
        private void OnDestroy()
        {
            UpdatePuzzleName();
        }
        
        private void UpdatePuzzleName()
        {
            currentPuzzle.name = _validNameChecker.GetPuzzleSaveName();
            puzzleSelectPort.selectedBox.UpdateName();
            _validNameChecker.ResetUserEntered();
        }

        private void OnSelectPuzzleBox()
        {
            currentPuzzle = puzzleSelectPort.selectedPuzzle;
            _validNameChecker.SetPlaceHolder(currentPuzzle.name);
            
            if (lastSelectedPuzzleID == "" || currentPuzzle.id != lastSelectedPuzzleID)
            {
                popupBox.Clear();
                popupBox.SetData(currentPuzzle);
            }
        
            float progression = currentPuzzle.difficulty >= (int) PuzzleDifficulty.Impossible
                ? 0
                : currentPuzzle.progression;
            progressionIcon.SetProgression(progression);
            
            difficultyIcon.SetDifficulty(currentPuzzle.difficulty);
            
            lastSelectedPuzzleID = currentPuzzle.id;
        }

        public void OnPlayButtonPressed()
        {
            ClosePopup();
            puzzleSelectPort.SelectAndLoad(currentPuzzle);
        }
        
        public void OnDeleteButtonPressed()
        {
            PopupWindowManager.instance.CreateConfirmPopupWindow(deletePopupDataObject, DeletePuzzle);
        }
        
        public void OnRestartButtonPressed()
        {
            PopupWindowManager.instance.CreateConfirmPopupWindow(restartPopupDataObject, RestartPuzzle);
        }

        void DeletePuzzle()
        {
            ClosePopup();
            SaveManager.TryDeletePuzzle(currentPuzzle);
        } 
        
        private void RestartPuzzle()
        {
            ClosePopup();
            SaveManager.RestartPuzzle(currentPuzzle);
        }

        private void ClosePopup()
        {
            PopupWindowManager.instance.ClosePopup(gameObject);
        }
    }
}