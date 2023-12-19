using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumUtility
{
    public static T GetEnumValue<T>(int value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }
}

public class DifficultyIcon : MonoBehaviour
{
    [SerializeField] TileColors difficultyColors;
    [SerializeField] ExplanationText _explanationText;

    private PuzzleDifficulty _difficulty = PuzzleDifficulty.Simple;
    
    public void SetDifficulty(int difficultyInt)
    {
        var difficulty = EnumUtility.GetEnumValue<PuzzleDifficulty>(difficultyInt);
        SetDifficulty(difficulty);
    }
    
    public void SetDifficulty(PuzzleDifficulty difficulty)
    {
        string difficultyAsText = difficulty.ToString() ;
        _explanationText.SetText(difficultyAsText);
    }
}
