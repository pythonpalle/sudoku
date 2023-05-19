using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationTest : MonoBehaviour
{
    public int tilesCount;
    public int multCount;
    
    void Start()
    {
        int n = tilesCount;
        int k = multCount;

        List<int> tileList = new List<int>();
        for (int i = 0; i < n; i++)
        {
            tileList.Add(i);
        }
        
        int[] dataList = new int[k];
        

        List<List<int>> combinations = new List<List<int>>();
        FindAllCombinations(combinations,tileList, dataList, 0, n-1, 0, k);
        PrintAllCombinations(combinations);
    }

    private void PrintAllCombinations(List<List<int>> combinations)
    {
        foreach (List<int> intList in combinations)
        {
            string listString = "";
            foreach (int number in intList)
            {
                listString += $"{number}, ";
            }
            Debug.Log(listString);
        }
    }

    private void FindAllCombinations(List<List<int>> combinations, List<int> tileList, int[] tempList, 
        int start, int end, int index, int k)
    {
        if (index >= k)
        {
            combinations.Add(new List<int>(tempList));
            return;
        }
        
        for (int i = start; ( (i <= end) && (end - i + 1 >= k - index)); i++)
        {
            tempList[index] = tileList[i];
            FindAllCombinations(combinations, tileList, tempList, i + 1, end, index + 1, k);
        }
    }
}
