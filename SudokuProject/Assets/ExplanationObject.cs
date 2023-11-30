using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/ExplanationObject")]
public class ExplanationObject : ScriptableObject
{
    public ExplanationPopup ExplanationPopupPrefab;
    
    [HideInInspector]public ExplanationPopup ExplanationPopupInstance;
    public bool HasSpawnedObject => ExplanationPopupInstance != null;
    
    [HideInInspector] public Canvas Canvas;
}