using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportBehaviour : MonoBehaviour
{
    [SerializeField] private ImportObject importObject;
    [SerializeField] private TMP_InputField inputField;

    private string seedString = string.Empty;

    private bool enterButtonIsPressed => Input.GetKeyDown(KeyCode.Return);

    public void OnInputEnter(string seedString)
    {
        this.seedString = seedString;
    }

    // public void SetFieldSelected()
    // {
    //     importObject.isSelected = true;
    // }
    //
    // public void SetFieldDeselected()
    // {
    //     importObject.isSelected = false;
    // }

    private void Update()
    {
        if (enterButtonIsPressed)
        {
            TryImportSeed();
        }
    }

    public void TryImportSeed()
    {
        Debug.Log("Try import seed: " + seedString);

        bool validSeed = SeedIsValid(out string error);
        if (validSeed)
        {
            SudokuGrid9x9 grid = ConvertSeedToGrid();
            grid.PrintGrid();
            
            EventManager.ImportGrid(grid);
            
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(error);
        }
    }

    private SudokuGrid9x9 ConvertSeedToGrid()
    {
        SudokuGrid9x9 grid = new SudokuGrid9x9(false);

        for (int i = 0; i < seedString.Length; i++)
        {
            var digitChar = seedString[i];
            
            if (digitChar == '0' || digitChar == ' ')
                continue;
            
            int row = i / 9;
            int col = i % 9;

            int digitNumber = (int)Char.GetNumericValue(digitChar);
            
            grid.SetNumberToIndex(row, col, digitNumber);
        }

        return grid;
    }

    private bool SeedIsValid(out string errorMessage)
    {
        errorMessage = String.Empty;
        
        if (seedString.Length > 81)
        {
            seedString = seedString.Substring(0, 81);
            Debug.Log("Seed was too long, capped it at 81.");
        }
        
        foreach (var digit in seedString)
        {
            if (!Char.IsDigit(digit) && digit != ' ')
            {
                errorMessage = "Seed must only contain digits and spaces!";
                return false;
            }
        }

        return true;
    }
}
