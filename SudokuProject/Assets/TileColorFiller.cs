using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorFiller : MonoBehaviour
{
    public List<FillTileSection> sections;
    public TileColors tileColors;
    
    [SerializeField] ColorObject contradictionColor;
    [SerializeField] ColorObject baseColor;
    
    

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SetTileColors(List<int> colorMarks, bool contradicted)
    {
        int colorCount = colorMarks.Count;
        
        if (colorCount <= 1)
        {
            HandleOneColorFill(colorMarks, colorCount, contradicted);
            return;
        }
        else
        {
            float deltaFill = 1.0f / colorCount;
            float sectionFill = deltaFill;

            for (int i = 0; i < colorCount; i++)
            {
                sections[i].gameObject.SetActive(true);
                var color = GetColorForSection(colorMarks, i, contradicted);
                
                sections[i].SetFill(color, sectionFill);
                sectionFill += deltaFill;
            }
        }
        
        for (int i = colorCount + 1; i < sections.Count; i++) 
        {
            sections[i].gameObject.SetActive(false);
        }
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
            sections[0].SetFill(color, 1f);
        }
        else if (colorCount == 1)
        {
            Color color = GetColorForSection(colorNumbers, 0, isContradicted);
            sections[0].SetFill(color, 1f);
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
