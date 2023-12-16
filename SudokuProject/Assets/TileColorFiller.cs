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
    //private Color[] pixelsBeforeRed;
    // public List<int> currentColorNumbers = new List<int>();
    // public List<int> testColors = new List<int>();
    
    //public bool isContradicted;

    public ImageMerger merger;

    // private void RemoveAllColors()
    // {
    //     currentColorNumbers.Clear();
    // }
    //
    // private void AddOrRemoveColor(int number)
    // {
    //     if (currentColorNumbers.Contains(number))
    //         currentColorNumbers.Remove(number);
    //     else
    //         currentColorNumbers.Add(number);
    // }
    

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SetTileColors(List<int> colorMarks, bool contradicted)
    {
        int colorCount = colorMarks.Count;
        Debug.Log($"Colors: {colorCount}");
        
        if (colorCount <= 1)
        {
            HandleOneColorFill(colorMarks, colorCount, contradicted);
            return;
        }
        else
        {
            //currentColorNumbers.Sort();

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
        
        for (int i = colorCount + 1; i < 9; i++)
        {
            sections[i].gameObject.SetActive(false);
        }

    }

    // private List<FillTileSection> GetActiveSections()
    // {
    //     List<FillTileSection> activeSections = new List<FillTileSection>();
    //     for (int i = 0; i < currentColorNumbers.Count; i++)
    //     {
    //         activeSections.Add(sections[i]);
    //     }
    //
    //     return activeSections;
    // }

    private void HandleOneColorFill(List<int> colorNumbers, int colorCount, bool isContradicted)
    {
        Debug.Log($"Color count: {colorCount}");
        
        if (colorCount == 0)
        {
            //sections[0].gameObject.SetActive(true);

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
        Debug.Log($"Section index: {sectionIndex}");
        
        int tileIndex = currentColorNumbers[sectionIndex] - 1;
        Debug.Log($"Tile Index: {tileIndex}");


        Color color = tileColors.Colors[tileIndex];
        if (isContradicted)
        {
            color = new Color(color.r * 3, color.g, color.b) * 0.75f;
        }
        return color;
    }

    // private void RemoveContradiction()
    // {
    //     if (!isContradicted) return;
    //
    //     isContradicted = false;
    // }
    //
    // private void SetContradiction()
    // {
    //     if (isContradicted) return;
    //
    //     isContradicted = true;
    // }

    // private void Update() 
    // {
    //     for (int number = 1; number <= 9; number++)
    //     {
    //         if (InputManager.NumberKeyDown(number))
    //         {
    //             AddOrRemoveColor(number);
    //             SetTileColors();
    //         }
    //     }
    //
    //     if (InputManager.RemoveButtonIsPressed)
    //     {
    //         RemoveAllColors();
    //         SetTileColors();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         SetContradiction();
    //         SetTileColors();
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         RemoveContradiction();
    //         SetTileColors();
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         currentColorNumbers = new List<int>(testColors);
    //         SetTileColors();
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.M))
    //     {
    //         SetTileColors();
    //         merger.Merge(GetActiveSections());
    //     }
    // }
}
