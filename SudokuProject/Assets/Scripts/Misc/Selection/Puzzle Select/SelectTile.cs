using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        
        [Header("Candidate Add On")]
        [SerializeField] private RawImage candidateImageOverlay;
        
        public RectTransform RectTransform => rectTransform;
        
        private static float defaultCenterSize = 4.2f;  
        private static float defaultDigitSize = 10f;

        private void OnDestroy()
        {
            if (candidateImageOverlay)
                Destroy(candidateImageOverlay.gameObject);  
        }

        public void SetDigit(int number, bool permanent)
        {
            string text = number.ToString();

            SetDigitText(permanent, text);
        }

        public void ResetDigit()
        {
            SetDigitText(false, "");
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
        
        public void UpdateBackgroundColor(Color color)
        {
            colorFiller.SetBaseColor(color);
        }

        public void ResetHintDisplayInfo()
        {
            colorFiller.ResetBaseColor();
            candidateImageOverlay.gameObject.SetActive(false);
        }

        public void AddObjectAroundCandidate(int candidate, Texture2D texture, Color color, float scaleFactor)
        {
            StartCoroutine(PlaceObjectAroundCandidateRoutine(candidate, texture, color, scaleFactor));
        }

        private bool hasPlacedObject = false;

        private IEnumerator PlaceObjectAroundCandidateRoutine(int candidate, Texture2D texture, Color color, float scaleFactor)
        {
            if (!hasPlacedObject)
                yield return new WaitForEndOfFrame();
            
            var candidateText = candidateTexts[candidate - 1];
            
            candidateImageOverlay.gameObject.SetActive(true);
            
            // sets the overlay on top of everything else
            var overlayTransform = candidateImageOverlay.transform;
            overlayTransform.SetParent(transform.root, true);
            overlayTransform.SetAsLastSibling();
            
            overlayTransform.position = candidateText.transform.position;
            
            overlayTransform.localScale = Vector3.one * scaleFactor;

            //candidateImageOverlay.rectTransform.rect.size = candidateText.GetComponent<RectTransform>().sizeDelta;
            
            candidateImageOverlay.texture = texture;        
            candidateImageOverlay.color = color;

            hasPlacedObject = true;
        }
    }
}