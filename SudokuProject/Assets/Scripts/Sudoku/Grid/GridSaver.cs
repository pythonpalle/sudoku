using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saving;
using UnityEngine;

public class GridSaver : MonoBehaviour, IPopulatePuzzleData, ILoadPuzzleData
{
    private CommandManager _commandManager => CommandManager.instance;
    
    [SerializeField] private GridPort _gridPort;
    [SerializeField] private GeneratorPort generatorPort;
    [SerializeField] private SaveRequestPort requestPort;
    [SerializeField] private DifficultyObject _difficultyObject;

    private PuzzleSaveInfo puzzleSaveInfo = new PuzzleSaveInfo();

    private void OnEnable()
    {
        // way of debbuging directly from the Game Scene without first selecting a puzzle from the Select scene
        if (!SaveManager.HasCreatedPuzzleData && generatorPort.GenerationType != GridGenerationType.empty)
        {
            string errorMsg = "Puzzle save not found when initializing GridSaver!";
            if (Application.isEditor)
            {
                LoadFirstPuzzleOrNew();
                Debug.LogWarning(errorMsg);
            }
            else
            {
                Debug.LogError(errorMsg);
            }
        }
        
        AddListenersToSaveManager();

        if (generatorPort.GenerationType == GridGenerationType.loaded)
        {
            Invoke("LoadCurrentPuzzle", 0.02f);
        }
    }

    private void LoadFirstPuzzleOrNew()
    {
        if (SaveManager.TrySetCurrentUserData(out UserSaveData data))
        {
            if (data != null && data.puzzles.Any())
            {
                SaveManager.SetCurrentPuzzle(data.puzzles.First());
            }
        }    
    }

    private void OnDisable()
    {
        RemoveListenersFromSaveManager();
    }
    
    private void LoadCurrentPuzzle()
    {
        SaveManager.LoadCurrentPuzzle();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (Application.isPlaying) return;
        
        if (hasFocus) return;

        if (generatorPort.isGenerating) return;

        Debug.Log("App loses focus, save data!");
        requestPort.Location = SaveRequestLocation.ExitGameButton;

        var generationType = generatorPort.GenerationType;

        if (SaveManager.HasCreatedPuzzleData)
        {
            SaveManager.TrySave(requestPort.Location,  generationType, true);
        }
        else
        {
            _gridPort.RequestGrid();
            
            PuzzleDifficulty difficulty = puzzleSaveInfo.GetDifficultySuggestion(generationType, _gridPort.grid, _difficultyObject.Difficulty);
            string name = puzzleSaveInfo.GetNameSuggestion(generationType, difficulty);
            SaveManager.TryCreateNewPuzzleSave(name, requestPort.Location, difficulty, generationType);
        }
        
    }

    public void PopulateSaveData(PuzzleDataHolder dataHolder, bool newSelfCreate)
    {
        _gridPort.RequestGrid();

        // set tile numbers, marks, permanents and contradictions
        int i = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++) 
            {
                var tile = _gridPort.tileBehaviours[row, col];
                
                dataHolder.numbers[i] = tile.number;
                dataHolder.permanent[i] = newSelfCreate || tile.Permanent;

                dataHolder.cornerMarks[i] = tile.CornerMarks;
                dataHolder.centerMarks[i] = tile.CenterMarks;
                dataHolder.colorMarks[i] = tile.ColorMarks;

                dataHolder.contradicted[i] = tile.Contradicted;

                i++;
            } 
        }
        
        // set progress
        float progression = 0;

        int totalTiles = 81;
        int permanents = 0;
        int correctlyFilled = 0;
        
        for (i = 0; i < totalTiles; i++)
        {
            if (dataHolder.permanent[i])
            {
               permanents++;
            }
            else
            {
                if (dataHolder.numbers[i] != 0 && !dataHolder.contradicted[i])
                {
                    correctlyFilled++;
                }
            }
        }

        int totalUnpermanents = totalTiles - permanents;
        if (totalUnpermanents == 0)
        {
            dataHolder.progression = 0;
        }
        else
        {
            progression = correctlyFilled / (float)(totalTiles - permanents);
            dataHolder.progression = progression;
        }
    }

    public void LoadFromSaveData(PuzzleDataHolder puzzleData)
    {
        GameStateManager.SetActive(false);
        
        // create grid, populate it with permanent numbers
        SudokuGrid9x9 grid = new SudokuGrid9x9(false);

        var numbers = puzzleData.numbers;
        var permanents = puzzleData.permanent;
        var contradictions = puzzleData.contradicted;

        for (int i = 0; i < 81; i++)
        {
            int number = numbers[i];
            
            if (!permanents[i] || number == 0)
                continue;
            
            int row = i / 9;
            int col = i % 9;
            TileIndex index = new TileIndex(row, col);

            grid.SetNumberToIndex(index, numbers[i]);
        }

        EventManager.GenerateGrid(grid);
        
        // Populate grid with non permanent digits and color, center and corner marks
        for (int i = 0; i < 81; i++)
        {
            // Get all marks for the tile, add them to grid
            Dictionary<EnterType, List<int>> marksForTile = GetAllMarksForTile(puzzleData, i);
            _commandManager.AddAllMarksToTile(i, marksForTile);
            
            // if digit for tile is not permanent, add it to grid
            int number = numbers[i];
            if (!permanents[i] || number == 0)
            {
                _commandManager.AddDigitToTile(i, number);
            }

            if (contradictions[i])
            {
                _commandManager.AddContradictionToTile(i);
            }
        }

        _gridPort.RequestStatusUpdate();
        GameStateManager.SetActive(true);
    }

    private Dictionary<EnterType, List<int>> GetAllMarksForTile(PuzzleDataHolder puzzleData, int i)
    {
        Dictionary<EnterType, List<int>> marks = new Dictionary<EnterType, List<int>>();
        
        marks.Add(EnterType.CenterMark, puzzleData.centerMarks[i]);
        marks.Add(EnterType.CornerMark, puzzleData.cornerMarks[i]);
        marks.Add(EnterType.ColorMark, puzzleData.colorMarks[i]);

        return marks;
    }

    void AddListenersToSaveManager()
    {
        SaveManager.AddLoadDataListener(this);
        SaveManager.AddPopulateDataListener(this);
    }

    void RemoveListenersFromSaveManager()
    {
        SaveManager.RemoveLoadDataListener(this);
        SaveManager.RemovePopulateDataListener(this);
    }
}