using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionHolder : MonoBehaviour
{
    public List<MenuOption> menuOptions = new List<MenuOption>();
    
    [SerializeField, ReadOnly] private MenuOption selectedOption;

    private void Start()
    {
        InitializeChildren();
    }

    private void InitializeChildren()
    {
        foreach (MenuOption menuOption in menuOptions)
        {
            menuOption.parent = this;
        }
    }

    public void Select(MenuOption menuOption)
    {
        selectedOption = menuOption;    
    }
    
    public void Deselect(MenuOption menuOption)
    {
        if (selectedOption == menuOption)
            selectedOption = null;
    }
}