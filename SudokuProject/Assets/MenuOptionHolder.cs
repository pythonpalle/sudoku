using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionHolder : MonoBehaviour
{
    public List<MenuOption> menuOptions = new List<MenuOption>();
    
    [SerializeField, ReadOnly] private MenuOption selectedOption;

    private void Awake()
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

    public void RequestSelect(MenuOption menuOption)
    {
        foreach (MenuOption other in menuOptions)
        {
            other.Deselect();
        }
        
        selectedOption = menuOption;
        menuOption.Select();
    }
    
    public void RequestDeselect(MenuOption menuOption)
    {
        if (selectedOption == menuOption)
            selectedOption = null;

        menuOption.Deselect();
    }
}