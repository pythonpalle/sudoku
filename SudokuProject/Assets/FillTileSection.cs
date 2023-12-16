using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class FillTileSection : MonoBehaviour
{
    public Image image;
    
    public void SetFill(Color color, float amount)
    {
        image.color = color;
        image.fillAmount = amount;
    }
}
