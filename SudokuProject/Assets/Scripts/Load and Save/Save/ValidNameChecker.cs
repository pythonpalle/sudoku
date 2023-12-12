using TMPro;
using UnityEngine;

namespace Saving
{
    public class ValidNameChecker : MonoBehaviour
    {
        [Header("User input")]
        [SerializeField] private TextMeshProUGUI placeHolderText;
        [SerializeField] private TextMeshProUGUI userEnterText;

        public string GetPuzzleSaveName()
        {
            return CheckForValidPuzzleName() ? userEnterText.text : placeHolderText.text;
        }
        
        bool CheckForValidPuzzleName()
        {
            string enteredString = userEnterText.text.Trim();
            
            // only 1 character and is invisible empty char
            if (enteredString.Length == 1 && (int) enteredString[0] == 8203)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(enteredString))
            {
                // String is not empty
                return true;
            }
            else
            {
                // String is empty or contains only spaces or has zero-width space
                return false;
            }
        }

        public void SetPlaceHolder(string placeHolderString)
        {
            placeHolderText.text = placeHolderString;
        }
    }
}