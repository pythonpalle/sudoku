using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileColoring : MonoBehaviour 
{
    public RawImage colorWheelImage; // Reference to the RawImage component representing the color wheel
    private Texture2D colorWheelTexture; // The texture representing the color wheel
    public TileColors tileColors;
    public ColorObject contradictionColor;

    public int sectionCount; // Keeps track of the number of sections

    public List<int> currentColorNumbers;
    public bool isContradicted;

    void Start() 
    {
        // Initialize color wheel texture with a single color (you can set this to any desired color)
        colorWheelTexture = new Texture2D(256, 256);
        Color baseColor = Color.white; // Default color for the wheel
        FillTextureWithColor(colorWheelTexture, baseColor);

        // Apply the texture to the RawImage component
        colorWheelImage.texture = colorWheelTexture;
    }

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SplitColorWheel(int number, bool removeAll)
    {
        // Create a new texture to hold the updated sections
        Texture2D newTexture = new Texture2D(colorWheelTexture.width, colorWheelTexture.height);
        newTexture.SetPixels(colorWheelTexture.GetPixels());


        if (!removeAll)
        {
            if (currentColorNumbers.Contains(number))
            {
                currentColorNumbers.Remove(number);
            }
            else
            {
                currentColorNumbers.Add((number));
            }
        }
        

        int indexNumber = number - 1;
        
        sectionCount = currentColorNumbers.Count;
        if (sectionCount == 0)
        {
            colorWheelImage.color = isContradicted ? contradictionColor.Color : Color.white;
        } 

        currentColorNumbers.Sort();
        float angleBetweenSections = 360f / sectionCount;

        // Center point of the texture
        Vector2 centerPoint = new Vector2(newTexture.width / 2, newTexture.height / 2);

        // Loop through and color each section with a different color in a radial manner
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                if (sectionCount == 0)
                {
                    newTexture.SetPixel(x, y, Color.white);
                    continue;
                }
                
                int sectionIndex = 0;
                
                if (sectionCount > 1)
                {
                    Vector2 currentPoint = new Vector2(x, y);
                    Vector2 vectorToPixel = currentPoint - centerPoint;

                    float angleToPixel = Mathf.Atan2(vectorToPixel.y, vectorToPixel.x) * Mathf.Rad2Deg;
                    if (angleToPixel < 0)
                    {
                        angleToPixel += 360;
                    }
                    
                    sectionIndex = Mathf.FloorToInt(angleToPixel / angleBetweenSections);
                }
                
                Color sectionColor = GetColorForSection(sectionIndex);
                newTexture.SetPixel(x, y, sectionColor);
            }
        }

        newTexture.Apply(); // Apply changes
        colorWheelTexture = newTexture; // Update the current texture

        // Apply the updated texture to the RawImage component
        colorWheelImage.texture = colorWheelTexture;
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

    // Helper method to get color for a specific section of the color wheel
    private Color GetColorForSection(int sectionIndex)
    {
        //Debug.Log($"Section index: {sectionIndex}");
        int tileIndex = currentColorNumbers[sectionIndex] - 1;
        //Debug.Log($"Tile index: {tileIndex}");

        return tileColors.Colors[tileIndex];
        
        float hue = (sectionIndex * 1.0f) / sectionCount; // Divide by sectionCount for even distribution
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    private void Update() 
    {
        for (int i = 1; i <= 9; i++)
        {
            if (InputManager.NumberKeyDown(i))
                SplitColorWheel(i, false);
        }

        if (InputManager.RemoveButtonIsPressed)
        {
            currentColorNumbers.Clear();
            SplitColorWheel(-1, true);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isContradicted = !isContradicted;
            UpdateContradictionTone();
        }
        
    }

    private void UpdateContradictionTone()
    {
        Color[] pixels = colorWheelTexture.GetPixels();

        float darknessModifier = 0.7f;
        float oneOverDarknessModifier = 1 / darknessModifier;

        float rednessModifier = 1.5f;
        float oneOverRednessModifier = 1 / rednessModifier;

        float darkness = isContradicted ? darknessModifier : oneOverDarknessModifier;
        float redness = isContradicted ? rednessModifier : oneOverRednessModifier;

        for (int i = 0; i < pixels.Length; i++)
        {
            // Decrease brightness by reducing each color component
            pixels[i] *= darkness; 
            
            float redComponent = pixels[i].r;
            redComponent *= redness; 
            redComponent = Mathf.Clamp01(redComponent);
            
            pixels[i] = new Color(redComponent, pixels[i].g, pixels[i].b, pixels[i].a);
        }
        
        colorWheelTexture.SetPixels(pixels);
        colorWheelTexture.Apply();
    }
}
