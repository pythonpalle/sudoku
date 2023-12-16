using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorFiller : MonoBehaviour
{
    public List<FillTileSection> sections;
    public TileColors tileColors;
    
    [SerializeField] ColorObject contradictionColor;
    [SerializeField] ColorObject baseColor;

    private Texture2D texture; // The texture representing the color wheel
    private Color[] pixelsBeforeRed;
    public List<int> currentColorNumbers = new List<int>();
    public List<int> testColors = new List<int>();
    
    public bool isContradicted;

    private void RemoveAllColors()
    {
        currentColorNumbers.Clear();
    }
    
    private void AddOrRemoveColor(int number)
    {
        if (currentColorNumbers.Contains(number))
            currentColorNumbers.Remove(number);
        else
            currentColorNumbers.Add(number);
    }
    

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SetTileColors()
    {
        int colorCount = currentColorNumbers.Count;
        if (colorCount <= 1)
        {
            HandleOneColorFill(colorCount);
        }
        else
        {
            currentColorNumbers.Sort();

            float deltaFill = 1.0f / colorCount;
            float sectionFill = deltaFill;

            for (int i = 0; i < colorCount; i++)
            {
                sections[i].gameObject.SetActive(true);
                var color = GetColorForSection(i);
                sections[i].SetFill(color, sectionFill);

                sectionFill += deltaFill;
            }
        }

        for (int i = colorCount + 1; i < 9; i++)
        {
            sections[i].gameObject.SetActive(false);
        }
    }

    private void HandleOneColorFill(int colorCount)
    {
        if (colorCount == 0)
        {
            Color color = isContradicted ? contradictionColor.Color : baseColor.Color;
            sections[0].SetFill(color, 1f);
        }
        else if (colorCount == 1)
        {
            Color color = GetColorForSection(0);
            sections[0].SetFill(color, 1f);
        }
    }


    private Color GetColorForSection(int sectionIndex)
    {
        int tileIndex = currentColorNumbers[sectionIndex] - 1;
        return tileColors.Colors[tileIndex];
    }

    private void RemoveContradiction()
    {
        if (!isContradicted) return;

        isContradicted = false;
    }

    private void SetContradiction()
    {
        if (isContradicted) return;

        isContradicted = true;
    }

    private void Update() 
    {
        for (int number = 1; number <= 9; number++)
        {
            if (InputManager.NumberKeyDown(number))
            {
                AddOrRemoveColor(number);
                SetTileColors();
            }
        }

        if (InputManager.RemoveButtonIsPressed)
        {
            RemoveAllColors();
            SetTileColors();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SetContradiction();
            SetTileColors();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveContradiction();
            SetTileColors();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentColorNumbers = new List<int>(testColors);
            SetTileColors();
        }
    }

}
