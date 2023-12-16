using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TileSectionController : MonoBehaviour
{
    public RawImage tileImage;
    public Material tileMaterialAsset; // Reference to the material of the tile
    public Material tileMaterialInstance; // Reference to the material of the tile
    public int sections => currentColorNumbers.Count;
    
    public List<int> currentColorNumbers = new List<int>();

    [SerializeField] TileColors tileColors;
    [SerializeField] ColorObject contradictionColor;
    [SerializeField] ColorObject baseColor;

    private Texture2D texture; // The texture representing the color wheel
    private Color[] pixelsBeforeRed;
    public List<int> testColors = new List<int>();
    public bool isContradicted;
    
    private int textureSize = 64;

    private void Awake()
    {
        if (tileMaterialAsset != null && tileColors != null)
        {
            tileMaterialInstance = Instantiate(tileMaterialAsset);
            tileImage.material = tileMaterialInstance;
            
            SetTileColors();
        }
    }

    private void SetTextures()
    {
        int currentColorsCount = currentColorNumbers.Count;

        // if (currentColorsCount == 0)
        // {
        //     tileMaterialInstance.color = isContradicted ? contradictionColor.Color : baseColor.Color;
        //     return;
        // }
        
        Color[] selectedColors = new Color[currentColorsCount];

        for (var index = 0; index < currentColorsCount; index++)
        {
            int number = currentColorNumbers[index] - 1;
            selectedColors[index] = (tileColors.Colors[number]);
        }

        Texture2D colorTexture = new Texture2D(currentColorsCount, 1);
        colorTexture.SetPixels(selectedColors);
        colorTexture.Apply();

        tileMaterialInstance.SetTexture("_Colors", colorTexture);
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
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SetContradiction();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveContradiction();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentColorNumbers = new List<int>(testColors);
            SetTileColors();
        }
    }
    
    
    void SetPixelsBeforeRed()
    {
        pixelsBeforeRed = texture.GetPixels(); // Store the original pixel data
    }

    private void RemoveAllColors()
    {
        currentColorNumbers.Clear();
        SetTileColors();
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
        currentColorNumbers.Sort();
        tileMaterialInstance.SetInt("_Sections", sections); // Set the number of sections for this tile's material    }
        SetTextures(); 
    }

    // Helper method to fill a texture with a specific color
    private void FillTextureWithColor(Texture2D texture, Color color)
    {
        Color[] colors = new Color[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color; 
        }
        texture.SetPixels(colors);
        texture.Apply();
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
        RestoreOriginalTexture();
    }
    
    private void RestoreOriginalTexture()
    {
        if (currentColorNumbers.Count == 0)
        {
            Color restoreColor = isContradicted ? contradictionColor.Color : baseColor.Color;
            FillTextureWithColor(texture, restoreColor); 
            return;
        }
        
        texture.SetPixels(pixelsBeforeRed);
        texture.Apply();
    }

    private void SetContradiction()
    {
        if (isContradicted) return;

        isContradicted = true;
        SetRedTone();
    }

    private void SetRedTone()
    {
        SetTileColors();
        
        return;
        
        SetPixelsBeforeRed();

        if (currentColorNumbers.Count == 0)
        {
            RestoreOriginalTexture();
            return;
        }
        
        Color[] pixels = texture.GetPixels();

        float darknessModifier = 0.7f;
        float rednessModifier = 3f;

        for (int i = 0; i < pixels.Length; i++)
        {
            // Decrease brightness by reducing each color component
            pixels[i] *= darknessModifier; 
            
            float redComponent = pixels[i].r;
            redComponent *= rednessModifier; 
            redComponent = Mathf.Clamp01(redComponent);
            
            pixels[i] = new Color(redComponent, pixels[i].g, pixels[i].b, pixels[i].a);
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
    }

}