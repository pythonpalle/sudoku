using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Assertions;

public class TileColoring : MonoBehaviour 
{
    public RawImage tileImage; // Reference to the RawImage component representing the color wheel
    [SerializeField] TileColors tileColors;
    [SerializeField] ColorObject contradictionColor;
    [SerializeField] ColorObject baseColor;

    private Texture2D texture; // The texture representing the color wheel
    private Color[] pixelsBeforeRed;
    public List<int> currentColorNumbers = new List<int>();
    public List<int> testColors = new List<int>();
    public bool isContradicted;
    
    private int textureSize = 64;
    
    void Start()
    {
        tileImage.material = null;
        
        texture = new Texture2D(textureSize, textureSize);
        FillTextureWithColor(texture, baseColor.Color);
        tileImage.texture = texture;
    }

    void SetPixelsBeforeRed()
    {
        pixelsBeforeRed = texture.GetPixels(); // Store the original pixel data
    }

    private void RemoveAllColors()
    {
        currentColorNumbers.Clear();
        RestoreOriginalTexture();
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
            SetSingleColor(colorCount);
            return;
        }
        
        currentColorNumbers.Sort();

        // Create a new texture to hold the updated sections
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        newTexture.SetPixels(texture.GetPixels());
        
        float angleBetweenSections = 360f / colorCount;

        // Center point of the texture
        Vector2 centerPoint = new Vector2(newTexture.width / 2, newTexture.height / 2);

        // Loop through and color each section with a different color in a radial manner
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                int sectionIndex = GetSectionIndex(x, y, colorCount, angleBetweenSections, centerPoint);
                Color sectionColor = GetColorForSection(sectionIndex);
                newTexture.SetPixel(x, y, sectionColor);
            }
        }

        newTexture.Apply(); // Apply changes
        texture = newTexture; // Update the current texture

        // Apply the updated texture to the RawImage component
        tileImage.texture = texture;
        
        if (isContradicted)
            SetRedTone();
    }

    private int GetSectionIndex(int x, int y, int colorCount, float angleBetweenSections, Vector2 centerPoint)
    {
        if (colorCount == 2)
        {
            return y > x ? 0 : 1;
        }
        else
        {
            Vector2 currentPoint = new Vector2(x, y);
            Vector2 vectorToPixel = currentPoint - centerPoint;
                
            float angleToPixel = Mathf.Atan2(vectorToPixel.y, vectorToPixel.x) * Mathf.Rad2Deg;
            if (angleToPixel < 0)
            {
                angleToPixel += 360;
            }
            
            int index = Mathf.FloorToInt(angleToPixel / angleBetweenSections);

            Assert.IsTrue(index < 9);
            return index;
            
        }
    }
    
    private void SetSingleColor(int colorCount)
    {
        if (colorCount == 0)
        {
            RestoreOriginalTexture();
        } 
        else if (colorCount == 1)
        {
            int colorIndex = currentColorNumbers[0] - 1;
            Color color = tileColors.Colors[colorIndex];
        
            FillTextureWithColor(texture, color);

            if (isContradicted)
                SetRedTone();
        }
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

    
}
