using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PuzzleSelect
{
    public class SelectTile : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private TextMeshContainer cornerTextPrefab;
        private TextMeshContainer cornerTextInstance;

        [Header("Marks")]
        [SerializeField] private TextMeshProUGUI digitText;
        [SerializeField] private TileColorFiller colorFiller;
        
        [Header("Colors")]
        [SerializeField] private ColorObject permanentColor;
        [SerializeField] private ColorObject markColor;

        [Header("Misc")] 
        [SerializeField] private RectTransform rectTransform;
        
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

            cornerTextInstance = Instantiate(cornerTextPrefab, transform);
            cornerTextInstance.RectTransform.sizeDelta = rectTransform.sizeDelta;
            cornerTextInstance.TextMesh.text = MarkClass.GetCornersAsString(cornerMarks);
        } 
        
        public void SetColorMarks(List<int> colorMarks, bool contradicted)
        {
            colorFiller.SetTileColors(colorMarks, contradicted);
        } 

        public void Reset()
        {
            if (cornerTextInstance)
            {
                cornerTextInstance.TextMesh.text = "";
            }
            
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
                digitText.color = markColor.Color;
                if (cornerTextInstance) cornerTextInstance.TextMesh.color = markColor.Color;
            }
        }

        public void RemoveUnusedColors(int colorMarksLength)
        {
            colorFiller.RemoveUnusedSections(colorMarksLength);
        }
    }
}