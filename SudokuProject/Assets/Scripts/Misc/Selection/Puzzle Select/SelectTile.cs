using System;
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
        [SerializeField] private GameObject candidateParent;
        [SerializeField] private List<TextMeshProUGUI> candidateTexts;
        
        private static float defaultCenterSize = 4.2f; 
        private static float defaultDigitSize = 10f; 

        public void SetDigit(int number, bool permanent)
        {
            string text = number.ToString();

            SetDigitText(permanent, text);
        }

        private void SetDigitText(bool permanent, string text)
        {
            digitText.text = text;
            digitText.color = permanent ? permanentColor.Color : markColor.Color;
        }

        public void SetCandidatesDigit(HashSet<int> digits)
        {
            string text = "";
            
            candidateParent.SetActive(true);
            
            for (int digit = 1; digit <= 9; digit++)
            {
                bool hasCandidate = digits.Contains(digit);
                
                var candidateText = candidateTexts[digit - 1];
                
                candidateText.text = hasCandidate ? digit.ToString() : string.Empty;
                candidateText.color = markColor.Color;
            }
        }
        
        public void HideCandidates()
        {
            candidateParent.SetActive(false);
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
            {
                return;
            }

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

            digitText.text = "";
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