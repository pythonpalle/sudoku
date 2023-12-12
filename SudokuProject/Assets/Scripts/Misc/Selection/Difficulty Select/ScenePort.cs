using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sudoku/Ports/Scene Port")]
public class ScenePort : ScriptableObject
{
    public UnityAction OnCallLoadPuzzleSelectScene;
    public UnityAction OnCallLoadRandom;

    public void CallLoadPuzzleSelectScene()
    {
        OnCallLoadPuzzleSelectScene?.Invoke();
    }
    
    public void CallLoadRandom()
    {
        OnCallLoadRandom?.Invoke();
    }
}