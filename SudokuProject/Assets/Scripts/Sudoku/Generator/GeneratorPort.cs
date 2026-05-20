using UnityEngine;

[CreateAssetMenu(menuName = "Sudoku/GeneratorPort")]
public class GeneratorPort : ScriptableObject
{
    private GridGenerationType generationType;
    
    public GridGenerationType GetGenerationType() => generationType;

    public void SetGenerationType(GridGenerationType generationType)
    {
        this.generationType = generationType;
    }

    public bool isGenerating { get; set; }
}