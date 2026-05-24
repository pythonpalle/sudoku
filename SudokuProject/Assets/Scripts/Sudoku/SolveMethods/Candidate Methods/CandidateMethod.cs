
using System.Collections.Generic;
using System.Linq;

public struct MultiCombo
{
    public List<TileIndex> tileIndices;
    public HashSet<int> candidates;

    public MultiCombo(List<TileIndex> indices, HashSet<int> candidates)
    {
        tileIndices = indices;
        this.candidates = candidates;
    }
}

public struct CandidateSolveInformation
{
    public List<TileIndex> removalIndexes;
    public HashSet<int> candidateSet;

    // NYTT: Valfri visuell data för GUI-rendering
    public List<TileIndex> triggerIndexes;
    public List<TileIndex> fullHouseVisualIndexes; 
    public HouseType? houseType; 

    // Behåll dina gamla konstruktorer exakt som de är så ingenting går sönder!
    public CandidateSolveInformation(List<TileIndex> removalIndexes, HashSet<int> candidateSet)
    {
        this.removalIndexes = removalIndexes;
        this.candidateSet = candidateSet;
        this.triggerIndexes = null;
        this.fullHouseVisualIndexes = null;
        this.houseType = null;
    }
    
    public CandidateSolveInformation(List<TileIndex> removalIndexes, int candidate) 
        : this(removalIndexes, new HashSet<int> {candidate}) { }

    // NY: En extra konstruktor för när du VILL skicka med visuell data
    public CandidateSolveInformation(List<TileIndex> removalIndexes, HashSet<int> candidateSet, List<TileIndex> triggerIndexes, List<TileIndex> fullHouse, HouseType type)
    {
        this.removalIndexes = removalIndexes;
        this.candidateSet = candidateSet;
        this.triggerIndexes = triggerIndexes;
        this.fullHouseVisualIndexes = fullHouse;
        this.houseType = type;
    }

    public CandidateSolveInformation(List<TileIndex> removalIndexes, int candidate, List<TileIndex> triggerIndexes,
        List<TileIndex> fullHouse, HouseType type)
        : this(removalIndexes, new HashSet<int> {candidate}, triggerIndexes, fullHouse, type)
    {
    }
}

public abstract class CandidateMethod : SolveMethod
{
    public abstract bool TryFindCandidates(SudokuGrid9x9 grid, out CandidateSolveInformation solveInformation);
    
    public override string GetSolveDescription(SolutionStepData solutionStepData)
    {
        return $"{GetName} found at positions {solutionStepData.GetCandidateIndexesString()} for candidates {solutionStepData.GetCandidateDigitsString()}";
    }
 
    protected bool AllIndicesHaveSameRowCol(List<TileIndex> tileIndices, bool checkRow)
    {
        if (checkRow)
        {
            int tileRow = tileIndices[0].row;
            return tileIndices.All(tile => tile.row == tileRow);
        }
        else
        {
            int tileCol = tileIndices[0].col;
            return tileIndices.All(tile => tile.col == tileCol);
        }
    }
    
    protected bool ValidTile(SudokuTile compareTile, List<TileIndex> indices)
    {
        return !compareTile.Used && indices.All(index => index != compareTile.index);
    }
    
    protected void FindAllCombinations(List<List<TileIndex>> combinations, List<SudokuTile> tileList, SudokuTile[] tempList, 
        int start, int end, int index, int k)
    {
        // from https://www.geeksforgeeks.org/print-all-possible-combinations-of-r-elements-in-a-given-array-of-size-n/
        
        if (index == k)
        {
            if (ValidCombination(tempList, k))
            {
                List<TileIndex> indices = GetIndicesFromTiles(tempList);
                combinations.Add(indices);
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = tileList[i];
            FindAllCombinations(combinations, tileList, tempList, i + 1, end, index + 1, k);
        }
    }
    
    protected void FindAllCombinations( List<MultiCombo> combinations, List<int> numbers,
        Dictionary<int, List<TileIndex>> candidateCount, int[] tempList, int start, int end, int index, int k)
    {
        if (index >= k)
        {
            if (ValidCombination(tempList, k, candidateCount, out List<TileIndex> indices))
            {
                HashSet<int> candidates = new HashSet<int>(tempList);

                MultiCombo combo = new MultiCombo(indices, candidates);

                combinations.Add(combo);
                //combinations.Add((indices, candidates));
            }
            
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = numbers[i];
            FindAllCombinations(combinations, numbers, candidateCount, tempList, i + 1, end, index + 1, k);
        }
    }

    private bool ValidCombination(int[] tempList, int multCount, 
        Dictionary<int, List<TileIndex>> candidateCount, out List<TileIndex> combineIndicesForNumbers)
    {
        //List<TileIndex> combineIndicesForNumbers = new List<TileIndex>();
        combineIndicesForNumbers = new List<TileIndex>();

        foreach (int digit in tempList)
        {
            foreach (var tileIndex in candidateCount[digit])
            {
                if (!combineIndicesForNumbers.Contains(tileIndex))
                    combineIndicesForNumbers.Add(tileIndex);
            }
        }

        bool combineIndecesCountMatch = combineIndicesForNumbers.Count == multCount;
        return combineIndecesCountMatch;
    }

    private List<TileIndex> GetIndicesFromTiles(SudokuTile[] tileList)
    {
        List<TileIndex> indices = new List<TileIndex>();
        foreach (var tile in tileList)
        {
            indices.Add(tile.index);
        }

        return indices;
    }
    

    private bool ValidCombination(SudokuTile[] tempList, int multCount)
    {
        HashSet<int> sharedCandidates = new HashSet<int>();
        foreach (var tile in tempList)
        {
            sharedCandidates.UnionWith(tile.Candidates);
        }
        
        return sharedCandidates.Count == multCount;
    }
    
    protected bool TilesIntersect(TileIndex index1, TileIndex index2)
    {
        // same row
        if (index1.row == index2.row)
            return true;
        
        // same col
        if (index1.col == index2.col)
            return true;
        
        int boxRowTile1 = index1.row - index1.row % 3;
        int boxRowTile2 = index2.row - index2.row % 3;
        
        int boxColTile1 = index1.col - index1.col % 3;
        int boxColTile2 = index2.col - index2.col % 3;
        
        // same box
        return (boxRowTile1 == boxRowTile2
                && boxColTile1 == boxColTile2);
    }

    protected List<TileIndex> FindAllIndicesWithEntropy(SudokuGrid9x9 grid, int entropy)
    {
        List<TileIndex> entropyList = new List<TileIndex>();
        
        foreach (var tile in grid.Tiles)
        {
            if (!tile.Used && tile.Entropy == entropy)
                entropyList.Add(tile.index);
        }

        return entropyList;
    }
}