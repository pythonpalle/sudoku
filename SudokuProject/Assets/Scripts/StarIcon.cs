using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarIcon : MonoBehaviour
{
    [SerializeField] Image FillImage;

    public void SetColor(Color color)
    {
        FillImage.color = color;
    }
}
