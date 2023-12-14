using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Sudoku/Color/Color List Container")]
public class ColorListContainer : ScriptableObject
{
    public List<Color> Colors;
}

public class TileColoring : MonoBehaviour
{
    public SpriteRenderer tileSprite; // Reference to the SpriteRenderer component of your tile
    private Texture2D baseTexture; // The original base texture of the tile
    private Texture2D currentTexture; // The texture currently being displayed

    private int sectionCount = 1; // Keeps track of the number of sections
    
    [SerializeField] private ColorListContainer markColors;

    void Start()
    {
        // Initialize base texture with a single color (you can set this to any desired color)
        baseTexture = new Texture2D(256, 256);
        Color baseColor = Color.white; // Default color for the tile
        FillTextureWithColor(baseTexture, baseColor);

        // Set the base texture to the sprite renderer
        Sprite baseSprite = Sprite.Create(baseTexture, new Rect(0, 0, baseTexture.width, baseTexture.height), Vector2.zero);
        tileSprite.sprite = baseSprite;
        currentTexture = baseTexture;
    }

    // Method to split the texture into sections based on the number of colors added (like a color wheel)
    public void SplitTileColorWheel()
    {
        sectionCount++; // Increase the number of sections

        // Create a new texture to hold the updated sections
        Texture2D newTexture = new Texture2D(currentTexture.width, currentTexture.height);
        newTexture.SetPixels(currentTexture.GetPixels());

        float angleBetweenSections = 360f / sectionCount;

        // Center point of the texture
        Vector2 centerPoint = new Vector2(newTexture.width / 2, newTexture.height / 2);

        // Loop through and color each section with a different color in a radial manner
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                Vector2 currentPoint = new Vector2(x, y);
                Vector2 vectorToPixel = currentPoint - centerPoint;

                float angleToPixel = Mathf.Atan2(vectorToPixel.y, vectorToPixel.x) * Mathf.Rad2Deg;
                if (angleToPixel < 0)
                {
                    angleToPixel += 360;
                }

                int sectionIndex = Mathf.FloorToInt(angleToPixel / angleBetweenSections);
                Color sectionColor = GetColorForSection(sectionIndex);

                newTexture.SetPixel(x, y, sectionColor);
            }
        }

        newTexture.Apply(); // Apply changes
        currentTexture = newTexture; // Update the current texture

        // Apply the updated texture to the sprite renderer
        Sprite updatedSprite = Sprite.Create(currentTexture, new Rect(0, 0, currentTexture.width, currentTexture.height), Vector2.zero);
        tileSprite.sprite = updatedSprite;
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
        float hue = (sectionIndex * 1.0f) / sectionCount; // Divide by sectionCount for even distribution
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    private void Update() 
    {
        if (Input.GetKeyDown((KeyCode.Q)))
            SplitTileColorWheel();
    }
}
