using System;
using System.Collections;
using System.Collections.Generic;
using Command;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ImportBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GridPort gridPort;

    private string seedString = string.Empty;

    private bool enterButtonIsPressed => Input.GetKeyDown(KeyCode.Return);

    public void OnInputEnter(string seedString)
    {
        this.seedString = seedString;
    }

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


            List<int> allIndices = new List<int>();
            List<int> previous = new List<int>();
            List<int> imported = SeedToIntList(seedString);

            // TileBehaviour[,] tiles = new TileBehaviour[9, 9];
            // Array.Copy(gridPort.tileBehaviours, tiles, 81);

            TileBehaviour[,] tiles = gridPort.tileBehaviours;
            
            for (int i = 0; i < 81; i++)
            {
                allIndices.Add(i);

                int row = i / 9;
                int col = i % 9;
                
                previous.Add(tiles[row,col].number);
            }
            
            ImportCommand importCommand = new ImportCommand
            {
                effectedIndexes = allIndices,
                importedGridDigits = imported,
                previousGridDigits = previous
            };

            importCommand.Execute();
            //EventManager.ImportGrid(grid);
            
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(error);
        }
    }

    private List<int> SeedToIntList(string seedString)
    {
        List<int> seedList = new List<int>();
        for (var index = 0; index < 81; index++)
        {
            int digit = 0;
            
            if (seedString.Length < 81)
            {
                seedList.Add(digit);
                continue;
            }
            
            var character = seedString[index];

            if (character != ' ')
                digit = (int) Char.GetNumericValue(character);

            Assert.IsTrue(digit <= 9 && digit >= 0);

            seedList.Add(digit);
        }

        return seedList;
    }

    public void PasteSeed()
    {
        seedString = GUIUtility.systemCopyBuffer;
        inputField.text = seedString;
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
