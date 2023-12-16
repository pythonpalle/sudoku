using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageMerger : MonoBehaviour
{
    public RawImage targetImage; // Reference to the RawImage where the merged texture will be displayed

    public void Merge(List<Texture2D> textures)
    {
        int totalWidth = 0;
        int maxHeight = 0;
        
        // Calculate total width and maximum height of source textures
        foreach (var texture in textures)
        {
            totalWidth += texture.width;
            maxHeight = Mathf.Max(maxHeight, texture.height);
        }
        
        // Create a new texture to hold the merged images
        Texture2D mergedTexture = new Texture2D(totalWidth, maxHeight);
        
        int xOffset = 0;
        
        // Merge textures into the new texture
        foreach (Texture2D texture in textures)
        {
            Color[] pixels = texture.GetPixels();
            mergedTexture.SetPixels(xOffset, 0, texture.width, texture.height, pixels);
            xOffset += texture.width;
        }
        
        mergedTexture.Apply(); // Apply changes to the merged texture
        
        // Display the merged texture in the target RawImage
        targetImage.texture = mergedTexture;
    }
}