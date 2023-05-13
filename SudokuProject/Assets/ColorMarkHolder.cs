using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorMarkHolder : MonoBehaviour
{
    [SerializeField] private List<Image> colorImages;
    [SerializeField] private TileColors _tileColors;

    private bool hasGottenColorImages = false;

    private void Start()
    {
        GetColorImages();
    }

    private void GetColorImages()
    {
        if (hasGottenColorImages) return;
        
        colorImages = GetComponentsInChildren<Image>(true).ToList();
        hasGottenColorImages = true;
    }

    public void SetColors(List<int> colorMarks)
    {
        GetColorImages();
        
        for (int i = 0; i < colorImages.Count; i++)
        {
            SetColorAtIndex(i, colorMarks);
        }
    }

    private void SetColorAtIndex(int i, List<int> colorMarks)
    {
        int colorIndex = colorMarks[i] - 1;
        colorImages[i].color = _tileColors.Colors[colorIndex];
    }
}
