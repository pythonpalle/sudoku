using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MenuOption))]
public class ExpendOnSelect : MonoBehaviour
{
    public float expandSize = 1.1f;
    private float startSize = 1f;
    
    private MenuOption menuOption;

    private void Awake()
    {
        menuOption = GetComponent<MenuOption>();
        startSize = transform.localScale.x;
    }

    private void OnEnable()
    {
        menuOption.OnSelect.AddListener(OnSelect); 
        menuOption.OnDeselect.AddListener(OnDeselect); 
    }
    
    private void OnDisable()
    {
        menuOption.OnSelect.RemoveListener(OnSelect); 
        menuOption.OnDeselect.RemoveListener(OnDeselect); 
    }

    private void OnSelect()
    {
        transform.localScale = new Vector3(expandSize, expandSize, expandSize);
    }
    
    private void OnDeselect()
    {
        transform.localScale = new Vector3(startSize, startSize, startSize);
    }
}