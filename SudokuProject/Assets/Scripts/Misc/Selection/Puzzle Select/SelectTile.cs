using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectTile : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI digitText;
        [SerializeField] private TextMeshProUGUI cornerText;
        
        [SerializeField] private ColorObject permanentColor;
        [SerializeField] private ColorObject markColor;
        
        private static float defaultCenterSize = 4.2f; 
        private static float defaultDigitSize = 10f; 

        public void SetDigit(int number, bool permanent)
        {
            digitText.text = number.ToString();
            digitText.color = permanent ? permanentColor.Color : markColor.Color;
        } 

        public void SetCenters(List<int> centerMarks)
        {
            if (centerMarks.Count == 0)
                return;
            
            MarkClass.UpdateCenterString(centerMarks, defaultCenterSize, digitText);
        } 
        
        public void SetCorners(List<int> cornerMarks)
        {
            if (cornerMarks.Count == 0)
                return;

            cornerText.text = MarkClass.GetCornersAsString(cornerMarks);
        } 
        
        public void SetColorMarks(List<int> colorMarks)
        {
            
        } 

        public void Reset()
        {
            digitText.text = cornerText.text = "";
            digitText.fontSize = defaultDigitSize;
        }

        public void SetTextColor(bool permanent)
        {
            if (permanent)
            {
                digitText.color =  permanentColor.Color;
            }
            else
            {
                digitText.color = cornerText.color = markColor.Color;
            }
        }
    }
}