using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageMerger : MonoBehaviour
{
    public RawImage targetImage; // Reference to the RawImage where the merged texture will be displayed

    public void Merge(List<FillTileSection> sections)
    {
        int totalWidth = 0;
        int maxHeight = 0;
        
        // Calculate total width and maximum height of source textures
        foreach (var section in sections)
        {
            var rect = section.image.rectTransform.rect;
            totalWidth += (int)rect.width;
            maxHeight = Mathf.Max(maxHeight, (int)rect.height);
        }
        
        // Create a new texture to hold the merged images
        Texture2D mergedTexture = new Texture2D(totalWidth, maxHeight);
        
        int xOffset = 0;
        
        foreach (var section in sections)
        {
            // Get the current slice's texture
            Texture2D sliceTexture = section.image.sprite.texture;

            // Ensure the slice has a valid texture
            if (sliceTexture != null)
            {
                Color32[] pixels = sliceTexture.GetPixels32();
                mergedTexture.SetPixels32(xOffset, 0, sliceTexture.width, sliceTexture.height, pixels);
                xOffset += sliceTexture.width;
            }
        }

        mergedTexture.Apply(); // Apply changes to the merged texture

        // Display the merged texture in the target RawImage
        targetImage.texture = mergedTexture;
    }
}