using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectTile : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI digitText;

        private LoadTile _loadTile;

        private static float defaultCenterSize = 4.2f; 

        public void SetDigit(int number, Color color)
        {
            digitText.text = number.ToString();
            digitText.color = color;
        }

        public void SetCenters(List<int> centerMarks)
        {
            CenterMark.UpdateCenterString(centerMarks, defaultCenterSize, digitText);
        } 
        
        public void SetCorners(List<int> cornerMarks)
        {
            
        } 
        
        public void SetColors(List<int> colorMarks)
        {
            
        } 

        public void Reset()
        {
            digitText.text = "";
        }

        public void SetTextColor(Color color)
        {
            digitText.color = color;
        }
    }
}

public class CenterMark
{
    private static int maximumMarksForDefaultSize = 5;
    
    public  static void UpdateCenterString(List<int> centerMarks, float defaultSize, TextMeshProUGUI centerText)
    {
        centerMarks.Sort();

        int centerMarkCount = centerMarks.Count;
        float centerStringSize = defaultSize;

        // decreasing font size to make sure all numbers fit in size
        if (centerMarkCount > maximumMarksForDefaultSize)
        {
            int difference = centerMarkCount - maximumMarksForDefaultSize;
            float powBase = 0.87f;

            centerStringSize *= Mathf.Pow(powBase, difference);
        }

        string centerMarkString = string.Empty;
        foreach (var mark in centerMarks)
        {
            centerMarkString += mark.ToString();
        }

        centerText.text = centerMarkString;
        centerText.fontSize = centerStringSize;
    }
}