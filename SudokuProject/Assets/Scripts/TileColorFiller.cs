using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorFiller : MonoBehaviour
{
    [SerializeField] private FillTileSection fillSectionPrefab;
    [SerializeField] List<FillTileSection> sections;
    [SerializeField] RectTransform rectTransform;
    
    [SerializeField] TileColors tileColors;
    [SerializeField] ColorObject contradictionColor;
    [SerializeField] ColorObject baseColor;

    private int BaseIndex => sections.Count - 1;

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SetTileColors(List<int> colorMarks, bool contradicted)
    {
        int colorCount = colorMarks.Count;
        
        // for (int i = 0; i < colorCount; i++)
        for (int i = colorCount; i < sections.Count - 1; i++) 
        {
            //if (i == BaseIndex) continue;
            
            sections[i].gameObject.SetActive(false);
        }

        if (colorCount <= 1)
        {
            HandleOneColorFill(colorMarks, colorCount, contradicted);
            return;
        }
        else
        {
            float deltaFill = 1.0f / colorCount;
            float sectionFill = deltaFill;

            while (sections.Count < colorCount)
            {
                var section = Instantiate(fillSectionPrefab, transform);
                section.RectTransform.sizeDelta = rectTransform.sizeDelta;
                sections.Add(section);
            }

            for (int i = 0; i < colorCount; i++)
            {
                int index = sections.Count - i - 1;
                
                sections[index].gameObject.SetActive(true);
                var color = GetColorForSection(colorMarks, i, contradicted);
                sections[index].SetFill(color, sectionFill);
                sectionFill += deltaFill;
            }
        }
        
        // // for (int i = 0; i < colorCount; i++)
        // for (int i = colorCount + 1; i < sections.Count; i++) 
        // {
        //     sections[i].gameObject.SetActive(false);
        // }
    } 

    public void RemoveUnusedSections(int colorCount)
    {
        int maxCount = Math.Max(colorCount, 1); 
        while (sections.Count > maxCount)
        {
            var section = sections[^1];
            sections.RemoveAt(sections.Count - 1);
            Destroy(section.gameObject);
        }
    }


    private void HandleOneColorFill(List<int> colorNumbers, int colorCount, bool isContradicted)
    {
        if (colorCount == 0)
        {
            Color color = isContradicted ? contradictionColor.Color : baseColor.Color;
            sections[BaseIndex].SetFill(color, 1f);
        }
        else if (colorCount == 1)
        {
            Color color = GetColorForSection(colorNumbers, 0, isContradicted);
            sections[BaseIndex].SetFill(color, 1f);
        }
    }
    


    private Color GetColorForSection(List<int> currentColorNumbers, int sectionIndex, bool isContradicted)
    {
        int tileIndex = currentColorNumbers[sectionIndex] - 1;
        
        Color color = tileColors.Colors[tileIndex];
        if (isContradicted)
        {
            color = new Color(color.r * 3, color.g, color.b) * 0.75f;
        }
        return color;
    }
}
