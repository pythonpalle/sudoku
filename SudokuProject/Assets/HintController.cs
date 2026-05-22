using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleSelect;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    [Header("Ports")]
    [SerializeField] private GridPort gridPort;
    
    //[SerializeField] private List<SelectTile> tiles;
    [Header("Prefabs")]
    [SerializeField] private SelectTile selectTilePrefab;
    
    [Header("Parent")]
    [SerializeField] private RectTransform gridParent;

    [Header("Padding")]
    [SerializeField] private float paddingBetweenBoxes = 2f;
    [SerializeField] private float paddingBetweenCells = 5f;
    
    [Header("Add on Objects")]
    [SerializeField] private Texture2D circleTexture;

    private SudokuGrid9x9 _hintGrid;
    private WFCGridSolver _solver = new WFCGridSolver(PuzzleDifficulty.Extreme);
    
    private Dictionary<TileIndex, SelectTile> hintTiles = new Dictionary<TileIndex, SelectTile>();

    private List<GridLayoutGroup> boxGroups;
    
    private Stack<SudokuGrid9x9> gridStack = new Stack<SudokuGrid9x9>();
    
    public void OnEnable()
    {
        gridPort.RequestGrid();
        gridPort.RequestTiles();
        _hintGrid = new SudokuGrid9x9(gridPort.grid);
        
        // Starta en Coroutine istället för Invoke
        StartCoroutine(CreateInitialGridRoutine());
    }

    private IEnumerator CreateInitialGridRoutine()
    {
        // Vänta tills Unity har kört klart alla layout-beräkningar för denna frame
        yield return new WaitForEndOfFrame();
    
        CreateEmptyGrid();
        UpdateContents();

        TryFindNextSolutionStep();
    }

    private void TryFindNextSolutionStep()
    {
        if (TryFindProgression(_hintGrid, out SolutionStepData solutionStep))
        {
            ResetTileHintColors();
            
            if (!solutionStep.isDigitSolve)
                Debug.Log(solutionStep);
            if (solutionStep.isDigitSolve)
            {
                HandleDigitSolveHint(solutionStep);
            }
        }
    }

    private void ResetTileHintColors()
    {
        foreach (SelectTile selectTile in hintTiles.Values)
        {
            selectTile.ResetHintDisplayInfo();
        }
    }

    private void HandleDigitSolveHint(SolutionStepData solutionStep)
    {
        Debug.Log($"Place a {solutionStep.digit} at index {solutionStep.tileIndex}");
        
        var uiTile = GetUITile(solutionStep.tileIndex);
        uiTile.SetDigitSolveHint(solutionStep.digit);
        
        AddSolveCircleAroundDigit(uiTile, solutionStep.digit);
    }

    private void AddSolveCircleAroundDigit(SelectTile uiTile, int solutionStepDigit)
    {
        uiTile.AddObjectAroundCandidate(solutionStepDigit, circleTexture, Color.yellow);
    }

    private SelectTile GetUITile(TileIndex tileIndex)
    {
        return hintTiles[tileIndex];
    }

    private bool TryFindProgression(SudokuGrid9x9 gridCopy, out SolutionStepData solutionStep)
    {
        return _solver.TryFindProgression(gridCopy, out solutionStep);
    }

    private void CreateEmptyGrid()
    {
        float mazGridHeight = gridParent.rect.height;
        float mazGridWidth = gridParent.rect.width;
        
        float maxGridSize = Mathf.Min(mazGridHeight, mazGridWidth);
        
        Debug.Log("maxGridWidth: " + maxGridSize);
        
        float maxBoxSize = (maxGridSize - 2 * paddingBetweenBoxes) / 3;
        
        GridLayoutGroup gridLayoutGroup  = gridParent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = 3;
        gridLayoutGroup.cellSize = new Vector2(maxBoxSize, maxBoxSize);
        gridLayoutGroup.spacing = new Vector2(paddingBetweenBoxes, paddingBetweenBoxes);
        
        float maxCellSize = (maxBoxSize - 2 * paddingBetweenCells) / 3;
        
        boxGroups = new List<GridLayoutGroup>();
        
        for (int i = 0; i < 9; i++)
        {
            CreateBox(i, maxCellSize);
        }
    }

    private void CreateBox(int boxIndex, float maxCellSize)
    {
        var gridBox = new GameObject();
        gridBox.transform.SetParent(gridParent);
        gridBox.name = "Box" + boxIndex;
        var layoutGroup = gridBox.AddComponent<GridLayoutGroup>();

        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = 3;
        layoutGroup.cellSize = new Vector2(maxCellSize, maxCellSize);
        layoutGroup.spacing = new Vector2(paddingBetweenCells, paddingBetweenCells);

        for (int cellDeltaIndex = 0; cellDeltaIndex < 9; cellDeltaIndex++)
        {
            TileIndex tileIndex = GetTileIndex(boxIndex, cellDeltaIndex);
            
            var tile = Instantiate(selectTilePrefab, gridBox.transform);
            tile.gameObject.name = "Tile" + tileIndex;
            
            hintTiles.TryAdd(tileIndex, tile);
        }
        
        boxGroups.Add(layoutGroup);
    }

    private TileIndex GetTileIndex(int boxIndex, int cellDeltaIndex)
    {
        int boxRow = boxIndex / 3;
        int boxColumn = boxIndex % 3;
        
        int deltaCellRow = cellDeltaIndex / 3;
        int deltaCellColumn = cellDeltaIndex % 3;
        
        int cellRow = boxRow * 3 + deltaCellRow;
        int cellColumn = boxColumn * 3 + deltaCellColumn;
        
        return new TileIndex(cellRow, cellColumn);
    }

    void UpdateContents()
    {
        var gridTiles = gridPort.tileBehaviours;

        foreach (var kvp in hintTiles)
        {
            
            TileIndex tileIndex = kvp.Key;
            
            var tileBehaviourUI = gridTiles[tileIndex.row, tileIndex.col];
            
            // TODO: FIX THIS! Permanent should NOT be fetched from the UI tile. store it in the actual tile
            var permanent = tileBehaviourUI.Permanent;
            
            SelectTile selectTile = kvp.Value;
            
            var realTile = _hintGrid[tileIndex];

            if (realTile.Number != 0)
            {
                selectTile.SetDigit(realTile.Number, permanent);
                selectTile.HideCandidates();
            }
            else
            {
                if (!permanent) selectTile.ResetDigit();
                
                selectTile.SetCandidatesDigit(realTile.Candidates);
            }
        }
    }

    public void OnNextButtonClicked()
    {
        gridStack.Push(_hintGrid);
        
        _solver.TryProgressWithHumanMethods(); 
        _hintGrid = new SudokuGrid9x9(_solver.grid);

        UpdateContents();
        TryFindNextSolutionStep();
    }
    
    public void OnPreviousButtonClicked()
    {
        var prevGrid = gridStack.Pop();
        
        _hintGrid = prevGrid;
        _solver.SetGrid(prevGrid);
        
        UpdateContents();
        TryFindNextSolutionStep();
    }
}
