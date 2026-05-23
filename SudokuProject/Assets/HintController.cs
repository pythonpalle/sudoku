using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
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
    
    [Header("Color")]
    [SerializeField] private ColorObject hintSolveBgColor;
    [SerializeField] private ColorObject hintSolveCircleColor;
    [SerializeField] private ColorObject hintEffectedColor;
    [SerializeField] private ColorObject sharedHouseColor;

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
        SetDigitSolveColor(uiTile);
        
        var solveMethod = solutionStep.solveMethod;
        if (solveMethod is NakedSingle)
        {
            List<TileIndex> eliminatingPeersIndexes = GetEliminatingPeersNakedSingle(solutionStep.tileIndex, solutionStep.digit);
            List<SelectTile> eliminatingTiles = GetUITiles(eliminatingPeersIndexes);

            SetTriggeringTileColor(eliminatingTiles);
        }
        
        else if (solveMethod is HiddenSingle hiddenSingle)
        {
            var tileIndex = solutionStep.tileIndex;
            HouseType houseType = hiddenSingle.HouseType;
            var tileIndexesInSameHouse = GetPeersInHouse(tileIndex, houseType).Where(x => x != tileIndex).ToList(); 
            List<SelectTile> tilesInSameHouse = GetUITiles(tileIndexesInSameHouse);
            SetSharedHouseColor(tilesInSameHouse);
            
            List<TileIndex> getTileIndexesSeeingHouse = GetTileIndexesSeeingHouse(houseType, tileIndex, solutionStep.digit);
            List<SelectTile> laserUiTiles = GetUITiles(getTileIndexesSeeingHouse);

            
            List<LaserArrow> laserArrows = LaserArrow.CalculateLaserArrows(tileIndex, getTileIndexesSeeingHouse, houseType, _hintGrid);
            foreach (var laser in laserArrows)
            {
                Debug.Log($"Draw line from {laser.StartTile} to {laser.EndTile}");
            }
            
            
            SetTriggeringTileColor(laserUiTiles);

        }
        
        AddSolveCircleAroundDigit(uiTile, solutionStep.digit);
    }

    private List<TileIndex> GetTileIndexesSeeingHouse(HouseType houseType, TileIndex targetTileIndex, int digit)
{
    // 1. Hämta alla rutor i huset
    List<TileIndex> houseTiles = GetPeersInHouse(targetTileIndex, houseType);

    // 2. Hitta de tomma rutorna i huset som faktiskt MÅSTE elimineras
    // (Vi behöver inte eliminera rutor som redan har andra siffror, och inte målrutan)
    List<TileIndex> tilesToEliminate = houseTiles
        .Where(t => t != targetTileIndex && !_hintGrid[t].Used)
        .ToList();

    if (tilesToEliminate.Count == 0) return new List<TileIndex>();

    // 3. Kartlägg vilka laserrutor som kan eliminera vilka tomma rutor
    // Key: Laserrutan, Value: Lista på tomma rutor i huset som denna laser "ser"
    Dictionary<TileIndex, List<TileIndex>> laserCoverage = new Dictionary<TileIndex, List<TileIndex>>();

    foreach (var emptyTile in tilesToEliminate)
    {
        // Hitta alla rutor på spelplanen som "ser" denna specifika tomma ruta och har rätt siffra
        List<TileIndex> seers = GetAllSeersWithDigit(emptyTile, digit);

        foreach (var seer in seers)
        {
            // Laserrutan får inte tillhöra huset vi undersöker
            if (houseTiles.Contains(seer)) continue;

            if (!laserCoverage.ContainsKey(seer))
                laserCoverage[seer] = new List<TileIndex>();

            laserCoverage[seer].Add(emptyTile);
        }
    }

    // 4. Girig algoritm: Välj de lasrar som täcker flest kvarvarande tomma rutor
    List<TileIndex> minimalLaserTiles = new List<TileIndex>();
    HashSet<TileIndex> eliminatedSoFar = new HashSet<TileIndex>();

    while (eliminatedSoFar.Count < tilesToEliminate.Count)
    {
        TileIndex bestLaser = default;
        int maxNewCoverage = 0;

        foreach (var kvp in laserCoverage)
        {
            // Hur många NYA rutor eliminerar den här lasern som vi inte redan har täckt?
            int newCoverageCount = kvp.Value.Count(t => !eliminatedSoFar.Contains(t));

            if (newCoverageCount > maxNewCoverage)
            {
                maxNewCoverage = newCoverageCount;
                bestLaser = kvp.Key;
            }
        }

        // Om vi inte hittar någon laser som täcker mer (skydd mot ogiltiga Sudokus)
        if (maxNewCoverage == 0) break; 

        // Spara den bästa lasern och markera rutorna som eliminerade
        minimalLaserTiles.Add(bestLaser);
        foreach (var coveredTile in laserCoverage[bestLaser])
        {
            eliminatedSoFar.Add(coveredTile);
        }
    }

    return minimalLaserTiles;
}

// Enkel hjälpmetod för att hitta alla rutor som "ser" en specifik ruta och har rätt siffra
private List<TileIndex> GetAllSeersWithDigit(TileIndex index, int digit)
{
    List<TileIndex> seers = new List<TileIndex>();
    
    // Kombinera rad, kolumn och box för rutan
    var allPeers = GetRowPeers(index)
        .Concat(GetColPeers(index))
        .Concat(GetBoxPeers(index))
        .Distinct();

    foreach (var peer in allPeers)
    {
        if (_hintGrid[peer].Number == digit)
        {
            seers.Add(peer);
        }
    }
    return seers;
}

    private void SetTriggeringTileColor(List<SelectTile> eliminatingTiles)
    {
        UpdateBackgroundColor(eliminatingTiles, hintEffectedColor.Color);
    }
    
    private void SetSharedHouseColor(List<SelectTile> eliminatingTiles)
    {
        UpdateBackgroundColor(eliminatingTiles, sharedHouseColor.Color);
    }
    
    private static void UpdateBackgroundColor(List<SelectTile> eliminatingTiles, Color color)
    {
        foreach (var eliminatingTile in eliminatingTiles)
        {
            eliminatingTile.UpdateBackgroundColor(color);
        }
    }
    
    private void SetDigitSolveColor(SelectTile tile)
    {
        UpdateBackgroundColor(new List<SelectTile> { tile }, hintSolveBgColor.Color);
    }
    
    private static void UpdateBackgroundColor(SelectTile tile, Color color)
    {
        UpdateBackgroundColor(new List<SelectTile> { tile }, color);
    }

    private List<TileIndex> GetPeersInHouse(TileIndex tileIndex, HouseType houseType)
    {
        switch (houseType)
        {
            case HouseType.Row:
                return GetRowPeers(tileIndex);
            case HouseType.Column:
                return GetColPeers(tileIndex);
            case HouseType.Box:
                return GetBoxPeers(tileIndex);
        }
        
        throw new Exception($"Unknown house type {houseType}");
    }

    private List<SelectTile> GetUITiles(List<TileIndex> tileIndexes)
    {
        return tileIndexes
            .Where(hintTiles.ContainsKey)
            .Select(index => hintTiles[index])
            .ToList();
    }

    private List<TileIndex> GetEliminatingPeersNakedSingle(TileIndex nakedSingleIndex, int digit)
    {
        // 1. Get the 3 houses separately
        var rowPeers = GetRowPeers(nakedSingleIndex);
        var colPeers = GetColPeers(nakedSingleIndex);
        var boxPeers = GetBoxPeers(nakedSingleIndex);

        // 2. Try to find all 8 digits in a single house first (Highest Intuition)
        if (TryGetPeersFromHouse(colPeers, digit, out var colResult)) return colResult;
        if (TryGetPeersFromHouse(rowPeers, digit, out var rowResult)) return rowResult;
        if (TryGetPeersFromHouse(boxPeers, digit, out var boxResult)) return boxResult;

        // 3. Fallback: Prioritize intersections (tiles in the same Box AND Row/Col)
        List<TileIndex> prioritizedFallback = new List<TileIndex>();
    
        // Add box tiles that are also in the same row/col first
        var intersections = boxPeers.Where(t => rowPeers.Contains(t) || colPeers.Contains(t));
        prioritizedFallback.AddRange(intersections);
    
        // Add the rest of the unique peers
        var uniquePeers = rowPeers.Concat(colPeers).Concat(boxPeers).Distinct();
        prioritizedFallback.AddRange(uniquePeers.Where(t => !prioritizedFallback.Contains(t)));

        // Extract the final eliminating peers from the prioritized fallback list
        TryGetPeersFromHouse(prioritizedFallback, digit, out var finalResult);
        return finalResult;
    }
    
    private bool TryGetPeersFromHouse(IEnumerable<TileIndex> houseTiles, int targetDigit, out List<TileIndex> result)
    {
        result = new List<TileIndex>();
        HashSet<int> neededDigits = new HashSet<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        neededDigits.Remove(targetDigit);

        foreach (var tileIndex in houseTiles)
        {
            var tile = _hintGrid[tileIndex];
            if (neededDigits.Contains(tile.Number))
            {
                result.Add(tileIndex);
                neededDigits.Remove(tile.Number);
                if (neededDigits.Count == 0) return true; // Successfully found all 8 digits
            }
        }
        return false; // This group didn't contain all 8 digits
    }

    private List<TileIndex> GetRowPeers(TileIndex index) => 
        Enumerable.Range(0, 9).Select(i => _hintGrid[index.row, i].index).ToList();

    private List<TileIndex> GetColPeers(TileIndex index) => 
        Enumerable.Range(0, 9).Select(i => _hintGrid[i, index.col].index).ToList();

    private List<TileIndex> GetBoxPeers(TileIndex index)
    {
        List<TileIndex> box = new List<TileIndex>();
        int startRow = index.row - index.row % 3;
        int startCol = index.col - index.col % 3;
        for (int r = 0; r < 3; r++)
        for (int c = 0; c < 3; c++)
            box.Add(_hintGrid[startRow + r, startCol + c].index);
        return box;
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