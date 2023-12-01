using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/ExplanationObject")]
public class ExplanationObject : ScriptableObject
{
    public TextMeshContainer textMeshContainerPrefab;
    
    [HideInInspector]public TextMeshContainer textMeshContainerInstance;
    public bool HasSpawnedObject => textMeshContainerInstance != null;
    
    [HideInInspector] public Canvas Canvas;
}