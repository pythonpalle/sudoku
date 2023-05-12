using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorMarkHolder : MonoBehaviour
{
    [SerializeField] private List<Image> colorImages;

    private void Start()
    {
        colorImages = GetComponentsInChildren<Image>(true).ToList();
    }
}
